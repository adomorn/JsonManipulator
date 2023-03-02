

# JsonManipulator

**JsonManipulator** is a helper class for performing various operations on JSON data.

## Features

Convert a JSON string to a dictionary of strings, where the keys are the property names and the values are the corresponding values in the JSON object.
Convert a Dictionary<string, string?> to a JSON string representation.

## Usage

```csharp
    using System.Collections.Generic;
    
    var json = @"{
      ""name"": ""John"",
      ""age"": 30,
      ""address"": {
        ""street"": ""123 Main St"",
        ""city"": ""New York""
      },
      ""phoneNumbers"": [
        ""123-456-7890"",
        ""987-654-3210""
      ]
    }";
    
    var values = JsonHelper.GetValuesFromJsonString(json);
    
    foreach (var keyValuePair in values)
    {
      Console.WriteLine($"{keyValuePair.Key}: {keyValuePair.Value}");
    }
    
    // Output:
    // name: John
    // age: 30
    // address:street: 123 Main St
    // address:city: New York
    // phoneNumbers:0: 123-456-7890
    // phoneNumbers:1: 987-654-3210
    
    var dictionary = new Dictionary<string, string?>
    {
      { "name", "John" },
      { "age", "30" },
      { "address:street", "123 Main St" },
      { "address:city", "New York" },
      { "phoneNumbers:0", "123-456-7890" },
      { "phoneNumbers:1", "987-654-3210" }
    };
    
    var jsonString = JsonHelper.ToJsonString(dictionary);
    
    Console.WriteLine(jsonString);
    
    // Output:
    // {
    //   "name": "John",
    //   "age": "30",
    //   "address": {
    //     "street": "123 Main St",
    //     "city": "New York"
    //   },
    //   "phoneNumbers": [
    //     "123-456-7890",
    //     "987-654-3210"
    //   ]
    // }
```
## Requirements

.NET Standard 2.0, .Net 5
- System.Text.Json

.NET 6,.NET 7
- No Dependencies

##License

This project is licensed under the MIT License. See the  [LICENSE](https://github.com/adomorn/JsonManipulator/blob/master/License.txt) file for details.
