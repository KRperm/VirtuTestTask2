using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VirtuTestTask2
{
    public class BasePolicyConverter : JsonConverter<BasePolicy>
    {
        public override BasePolicy? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var basePolicy = new BasePolicy()
            {
                Insurer = new Person(),
                Vehicle = new Vehicle(),
            };

            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            if (JsonConverterHelper.CheckProperty(reader, "Insurer"))
            {
                reader.Read();
                basePolicy.Insurer = JsonSerializer.Deserialize<Person>(ref reader);
            }
            else
            {
                var firstName = JsonConverterHelper.ReadStringFromProperty(ref reader, "InsurerFirstName");
                var lastName = JsonConverterHelper.ReadStringFromProperty(ref reader, "InsurerLastName");
                basePolicy.Insurer.Name = $"{firstName} {lastName}";
            }

            var vehicle = new Vehicle();
            if (JsonConverterHelper.CheckProperty(reader, "Vehicle"))
            {
                reader.Read();
                basePolicy.Vehicle = JsonSerializer.Deserialize<Vehicle>(ref reader);
            }
            else
            {
                basePolicy.Vehicle.MarkName = JsonConverterHelper.ReadStringFromProperty(ref reader, "VehicleMark");
                basePolicy.Vehicle.ModelName = JsonConverterHelper.ReadStringFromProperty(ref reader, "VehicleModel");
            }

            var effectiveDatePropertyName = JsonConverterHelper.CheckProperty(reader, "DateBegin") 
                ? "DateBegin"
                : "EffectiveDate";
            basePolicy.EffectiveDate = JsonConverterHelper.ReadDateTimeFromProperty(ref reader, effectiveDatePropertyName);

            var expirationDatePropertyName = JsonConverterHelper.CheckProperty(reader, "DateEnd")
                ? "DateEnd"
                : "ExpirationDate";
            basePolicy.ExpirationDate = JsonConverterHelper.ReadDateTimeFromProperty(ref reader, expirationDatePropertyName);

            JsonConverterHelper.ReadAndCheckObjectEnd(ref reader);

            return basePolicy;
        }

        public override void Write(Utf8JsonWriter writer, BasePolicy value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

    public class PersonConverter : JsonConverter<Person>
    {
        public override Person? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var person = new Person();

            // Вариант 1
            if (JsonConverterHelper.CheckProperty(reader, "FirstName"))
            {
                var firstName1 = JsonConverterHelper.ReadStringFromProperty(ref reader, "FirstName");
                var lastName1 = JsonConverterHelper.ReadStringFromProperty(ref reader, "LastName");
                person.Name = $"{firstName1} {lastName1}";
                reader.Read();
                return person;
            }

            // Вариант 2
            var personType = JsonConverterHelper.ReadStringFromProperty(ref reader, "Type");
            if (personType != "Person")
                throw new JsonException();

            JsonConverterHelper.ReadAndCheckProperty(ref reader, "Person");

            reader.Read();
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var firstName2 = JsonConverterHelper.ReadStringFromProperty(ref reader, "InsurerFirstName");
            var lastName2 = JsonConverterHelper.ReadStringFromProperty(ref reader, "InsurerLastName");
            person.Name = $"{firstName2} {lastName2}";
            JsonConverterHelper.ReadAndCheckObjectEnd(ref reader);
            JsonConverterHelper.ReadAndCheckObjectEnd(ref reader);
            return person;
        }

        public override void Write(Utf8JsonWriter writer, Person value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

    public static class JsonConverterHelper
    {
        public static void ReadAndCheckObjectEnd(ref Utf8JsonReader reader)
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.EndObject)
                throw new JsonException();
        }

        public static void ReadAndCheckProperty(ref Utf8JsonReader reader, string expectedProperty)
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();
            if (reader.GetString() != expectedProperty)
                throw new JsonException();
        }

        public static bool CheckProperty(Utf8JsonReader reader, string expectedProperty)
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();
            return reader.GetString() == expectedProperty;
        }

        public static string ReadStringFromProperty(ref Utf8JsonReader reader, string property)
        {
            ReadAndCheckProperty(ref reader, property);

            reader.Read();
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException();

            return reader.GetString();
        }

        public static DateTime ReadDateTimeFromProperty(ref Utf8JsonReader reader, string property)
        {
            var dateString = ReadStringFromProperty(ref reader, property);
            return DateTime.Parse(dateString);
        }
    }
}
