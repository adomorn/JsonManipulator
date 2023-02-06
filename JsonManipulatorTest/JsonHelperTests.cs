using JsonManipulator;

namespace JsonManipulatorTest
{
    public class JsonHelperTests
    {
        [Fact]
        public void GetValuesFromJsonString_ValidJsonString_ReturnsExpectedDictionary()
        {
            // Arrange
            var json =
                "{\"firstName\":\"John\",\"lastName\":\"Doe\",\"age\":30,\"address\":{\"street\":\"123 Main St\",\"city\":\"Anytown\",\"state\":\"CA\"},\"phoneNumbers\":[\"555-555-1212\",\"555-555-1213\"]}";

            // Act
            var result = JsonHelper.GetValuesFromJsonString(json);

            // Assert
            var expected = new Dictionary<string, string?>
            {
                { "firstName", "John" },
                { "lastName", "Doe" },
                { "age", "30" },
                { "address:street", "123 Main St" },
                { "address:city", "Anytown" },
                { "address:state", "CA" },
                { "phoneNumbers:0", "555-555-1212" },
                { "phoneNumbers:1", "555-555-1213" },
            };
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetValuesFromJsonString_EmptyJsonObjectString_ReturnsEmptyDictionary()
        {
            // Act
            var result = JsonHelper.GetValuesFromJsonString("{}");

            // Assert
            var expected = new Dictionary<string, string?>();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToJsonString_ValidDictionary_ReturnsExpectedJsonString()
        {
            // Arrange
            var keyValuePairs = new Dictionary<string, string?>
            {
                { "firstName", "John" },
                { "lastName", "Doe" },
                { "age", "30" },
                { "address:street", "123 Main St" },
                { "address:city", "Anytown" },
                { "address:state", "CA" },
                { "phoneNumbers:0", "555-555-1212" },
                { "phoneNumbers:1", "555-555-1213" },
            };

            // Act
            var result = keyValuePairs.ToJsonString();

            // Assert
            var expected = @"{
  ""firstName"": ""John"",
  ""lastName"": ""Doe"",
  ""age"": 30,
  ""address"": {
    ""street"": ""123 Main St"",
    ""city"": ""Anytown"",
    ""state"": ""CA""
  },
  ""phoneNumbers"": [
    ""555-555-1212"",
    ""555-555-1213""
  ]
}";

            Assert.Equal(expected, result);
        }
    }
}