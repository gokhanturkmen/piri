using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Piri.Benchmarks.Models.Complex;
using Piri.Benchmarks.Models.Simple;
using Piri.Core;
using System.Reflection;
using IAutoMapper = AutoMapper.IMapper;
using IPiriMapper = Piri.Core.IMapper;

namespace Piri.Benchmarks
{
    //[ShortRunJob]
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class PiriBenchmarks
    {
        private IPiriMapper _piriMapper;
        private IAutoMapper _autoMapper;
        private Source _source;
        private Destination _destination;
        private OrderDto _orderDto;

        [GlobalSetup]
        public void Setup()
        {
            _piriMapper = PiriMapper.Create(config =>
            {
                config.AddMap<Source, Destination>(source =>
                {
                    var destination = new Destination
                    {
                        Id = source.Id,
                        Name = source.Name,
                        Description = source.Description
                    };
                    return destination;
                })
                .AddDefaultMappingFor(obj =>
                {
                    if (obj is CustomerDto customerDto)
                    {
                        return new CustomerViewModel
                        {
                            Name = customerDto.Name,
                            Email = customerDto.Email,
                            PhoneNumberFormatted = customerDto.PhoneNumber
                        };
                    }

                    throw new NotImplementedException();
                })
                .AddDefaultMappingFor(obj =>
                {
                    if (obj is AddressDto addressDto)
                    {
                        return new AddressViewModel
                        {
                            FullAddress = $"{addressDto.Street}, {addressDto.City}, {addressDto.State}, {addressDto.PostalCode}, {addressDto.Country}"
                        };
                    }

                    throw new NotImplementedException();
                })
                .AddDefaultMappingFor(obj =>
                {
                    if (obj is OrderItemDto orderItemDto)
                    {
                        return new OrderItemViewModel
                        {
                            ProductName = orderItemDto.ProductName,
                            QuantityAndPrice = $"{orderItemDto.Quantity} x {orderItemDto.Price}",
                            TotalItemPriceFormatted = (orderItemDto.Quantity * orderItemDto.Price).ToString("C")
                        };
                    }

                    throw new NotImplementedException();
                })
                .AddDefaultMappingFor(obj =>
                {
                    if (obj is OrderDto orderDto)
                    {
                        var orderViewModel = new OrderViewModel
                        {
                            OrderId = orderDto.OrderId.ToString(),
                            OrderDateFormatted = orderDto.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            Customer = _piriMapper.Map<CustomerViewModel>(orderDto.Customer),
                            TotalAmountFormatted = orderDto.TotalAmount.ToString("C"),
                            ShippingAddress = _piriMapper.Map<AddressViewModel>(orderDto.ShippingAddress),
                            BillingAddress = _piriMapper.Map<AddressViewModel>(orderDto.BillingAddress),
                            ShippingStatus = orderDto.IsShipped ? "Shipped" : "Not Shipped",
                            Items = orderDto.Items.Select(_piriMapper.Map<OrderItemViewModel>).ToList()
                        };
                        return orderViewModel;
                    }

                    throw new NotImplementedException();
                })
                .AddMap<Destination, Source>()
                //.EnableDefaultMapping()
                ;
            });

            _autoMapper = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Source, Destination>()
                    .ReverseMap();
                cfg.CreateMap<CustomerDto, CustomerViewModel>();
                cfg.CreateMap<AddressDto, AddressViewModel>()
                    .ForMember(dest => dest.FullAddress, opt => opt.MapFrom(src => $"{src.Street}, {src.City}, {src.State}, {src.PostalCode}, {src.Country}"));
                cfg.CreateMap<OrderItemDto, OrderItemViewModel>()
                    .ForMember(dest => dest.QuantityAndPrice, opt => opt.MapFrom(src => $"{src.Quantity} x {src.Price}"))
                    .ForMember(dest => dest.TotalItemPriceFormatted, opt => opt.MapFrom(src => (src.Quantity * src.Price).ToString("C")));
                cfg.CreateMap<OrderDto, OrderViewModel>()
                    .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId.ToString()))
                    .ForMember(dest => dest.OrderDateFormatted, opt => opt.MapFrom(src => src.OrderDate.ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => _autoMapper.Map<CustomerViewModel>(src.Customer)))
                    .ForMember(dest => dest.TotalAmountFormatted, opt => opt.MapFrom(src => src.TotalAmount.ToString("C")))
                    .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => _autoMapper.Map<AddressViewModel>(src.ShippingAddress)))
                    .ForMember(dest => dest.BillingAddress, opt => opt.MapFrom(src => _autoMapper.Map<AddressViewModel>(src.BillingAddress)))
                    .ForMember(dest => dest.ShippingStatus, opt => opt.MapFrom(src => src.IsShipped ? "Shipped" : "Not Shipped"))
                    .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(_autoMapper.Map<OrderItemViewModel>).ToList()));
            }).CreateMapper();

            _source = new Source
            {
                Id = 1,
                Name = "Source",
                Description = "This is a source object."
            };

            _destination = new Destination
            {
                Id = 1,
                Name = "Destination",
                Description = "This is a destination object."
            };

            _orderDto = new OrderDto
            {
                OrderId = 1,
                OrderDate = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Customer = new CustomerDto
                {
                    CustomerId = 1,
                    Name = "Customer",
                    Email = "",
                    PhoneNumber = ""
                },
                TotalAmount = 100,
                ShippingAddress = new AddressDto
                {
                    Street = "Street",
                    City = "City",
                    State = "State",
                    PostalCode = "PostalCode",
                    Country = "Country"
                },
                BillingAddress = new AddressDto
                {
                    Street = "Street",
                    City = "City",
                    State = "State",
                    PostalCode = "PostalCode",
                    Country = "Country"
                },
                IsShipped = Random.Shared.Next(0, 2) == 1,
                Items = Enumerable.Range(0, 1000).Select(i => new OrderItemDto
                {
                    ItemId = i,
                    ProductName = $"Product {i}",
                    Quantity = i,
                    Price = i * 10
                }).ToList()
            };
        }

        [Benchmark]
        public Destination PiriMapperSimpleObjectExplicitly()
        {
            return _piriMapper.Map<Destination>(_source);
        }

        [Benchmark]
        public Source PiriMapperSimpleObjectImplicitly()
        {
            return _piriMapper.Map<Source>(_destination);
        }

        [Benchmark]
        public OrderViewModel PiriMapperComplexObjectExplicitly()
        {
            return _piriMapper.Map<OrderViewModel>(_orderDto);
        }

        [Benchmark]
        public Destination AutoMapperMapSimpleObject()
        {
            return _autoMapper.Map<Destination>(_source);
        }

        [Benchmark]
        public Source AutoMapperMapSimpleObjectReverse()
        {
            return _autoMapper.Map<Source>(_destination);
        }

        [Benchmark]
        public OrderViewModel AutoMapperMapComplexObject()
        {
            return _autoMapper.Map<OrderViewModel>(_orderDto);
        }

        [Benchmark]
        public Destination CustomMappingSimple()
        {
            return new Destination
            {
                Id = _source.Id,
                Name = _source.Name,
                Description = _source.Description
            };
        }

        [Benchmark]
        public Destination CustomMappingWithReflection()
        {
            var sourceType = _source.GetType();
            var destinationType = typeof(Destination);

            var newObject = Activator.CreateInstance<Destination>();
            foreach (var sourceProperty in sourceType.GetProperties().Where(p => p.CanRead))
            {
                var destinationProperty = destinationType.GetProperty(sourceProperty.Name, BindingFlags.SetProperty);
                if (destinationProperty == null
                    || sourceProperty.PropertyType != destinationProperty.PropertyType)
                {
                    continue;
                }
                destinationProperty.SetValue(newObject, sourceProperty.GetValue(_source));
            }

            foreach (var sourceField in sourceType.GetFields().Where(f => f.IsPublic))
            {
                var destinationField = destinationType.GetField(sourceField.Name, BindingFlags.Public | BindingFlags.SetField);
                if (destinationField == null
                    || sourceField.FieldType != destinationField.FieldType)
                {
                    continue;
                }

                destinationField.SetValue(newObject, sourceField.GetValue(_source));
            }

            return newObject;
        }

        [Benchmark]
        public OrderViewModel CustomMappingComplex()
        {
            return new OrderViewModel
            {
                OrderId = _orderDto.OrderId.ToString(),
                OrderDateFormatted = _orderDto.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Customer = new CustomerViewModel
                {
                    Name = _orderDto.Customer.Name,
                    Email = _orderDto.Customer.Email,
                    PhoneNumberFormatted = _orderDto.Customer.PhoneNumber
                },
                TotalAmountFormatted = _orderDto.TotalAmount.ToString("C"),
                ShippingAddress = new AddressViewModel
                {
                    FullAddress = $"{_orderDto.ShippingAddress.Street}, {_orderDto.ShippingAddress.City}, {_orderDto.ShippingAddress.State}, {_orderDto.ShippingAddress.PostalCode}, {_orderDto.ShippingAddress.Country}"
                },
                BillingAddress = new AddressViewModel
                {
                    FullAddress = $"{_orderDto.BillingAddress.Street}, {_orderDto.BillingAddress.City}, {_orderDto.BillingAddress.State}, {_orderDto.BillingAddress.PostalCode}, {_orderDto.BillingAddress.Country}"
                },
                ShippingStatus = _orderDto.IsShipped ? "Shipped" : "Not Shipped",
                Items = _orderDto.Items.Select(orderItemDto => new OrderItemViewModel
                {
                    ProductName = orderItemDto.ProductName,
                    QuantityAndPrice = $"{orderItemDto.Quantity} x {orderItemDto.Price}",
                    TotalItemPriceFormatted = (orderItemDto.Quantity * orderItemDto.Price).ToString("C")
                }).ToList()
            };

        }
    }
}
