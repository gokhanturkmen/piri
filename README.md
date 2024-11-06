# Piri
Simple object mapper for C#

## Overview
Piri is a lightweight and easy-to-use object mapper for C#. It allows you to map objects of one type to another with minimal configuration. This can be particularly useful when working with data transfer objects (DTOs) and domain models.

- Simple and intuitive API
- High performance
- Supports complex mappings
- Customizable mappings
- Fluent configuration

## Installation
You can install Piri via NuGet Package Manager. Run the following command in the Package Manager Console:
Install-Package Piri

## Getting Started
Let's say you have the following source and destination classes:

```csharp
public class Source
{
	public int Id { get; set; }
	public string Name { get; set; }
}

public class Destination
{
	public int Id { get; set; }
	public string FullName { get; set; }
}
```
You can map a Source object to a Destination object like this:
```csharp
using Piri.Core

public class Program
{
	public static void Main()
	{
		IMapper mapper = PiriMapper.Create(options => {
			options.AddMap<Source, Destination>(source => new Destination {
				Id = source.Id,
				FullName = source.Name
			});
		});
		var source = new Source { Id = 1, Name = "John Doe" };
		var destination = mapper.Map<Destination>(source);

		Console.WriteLine($"Id: {destination.Id}, FullName: {destination.FullName}");
	}
}
```


## Documentation
For more detailed documentation and advanced usage, please refer to the [official documentation](https://github.com/gokhanturkmen/piri/wiki).

## Contributing
We welcome contributions to Piri. If you would like to contribute, please fork the repository and submit a pull request. For major changes, please open an issue first to discuss what you would like to change.

## License
Piri is licensed under the MIT License. See the [LICENSE](LICENSE) file for more information.

## Contact
If you have any questions or feedback, feel free to reach out to us at [me@gturkmen.com](mailto:me@gturkmen.com).
