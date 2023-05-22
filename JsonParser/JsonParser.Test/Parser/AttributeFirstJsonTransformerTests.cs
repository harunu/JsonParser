using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class AttributeFirstJsonTransformerTests
    {
        [Test]
        public void WhenEmptyObjectThenEmptyObject()
        {
            // Arrange
            var input = new MemoryStream(Encoding.ASCII.GetBytes("{}"));

            // Act
            var output = AttributeFirstJsonTransformer.Transform(input);
            var actual = new StreamReader(output).ReadToEnd();

            // Assert
            Assert.AreEqual("{}", actual);
        }


        [Test]
        public void WhenNestedObjectThenAttributesFirst()
        {
            // Arrange
            var inputJson = @"{
                ""FirstName"":""Arthur"",
                ""LastName"":""Bertrand"",
                ""Address"":{
                    ""StreetName"":""Gedempte Zalmhaven"",
                    ""Number"":""4K"",
                    ""City"":{
                        ""Name"":""Rotterdam"",
                        ""Country"":""The Netherlands""
                    },
                    ""ZipCode"":""3011 BT""
                },
                ""Age"":35,
                ""Hobbies"":[
                    ""Fishing"",
                    ""Rowing""
                ]
            }";
            var expectedOutputJson = @"{""FirstName"":""Arthur"",""LastName"":""Bertrand"",""Age"":35,""Address"":{""StreetName"":""Gedempte Zalmhaven"",""Number"":""4K"",""ZipCode"":""3011 BT"",""City"":{""Name"":""Rotterdam"",""Country"":""The Netherlands""}},""Hobbies"":[""Fishing"",""Rowing""]}";

            var input = new MemoryStream(Encoding.ASCII.GetBytes(inputJson));

            // Act
            var output = AttributeFirstJsonTransformer.Transform(input);
            var actual = new StreamReader(output).ReadToEnd();

            // Assert
            Assert.AreEqual(expectedOutputJson, actual);
        }

        [Test]
        public void Transform_ReordersJsonAttributesButResultIsNotEqualToExpected()
        {
            // Arrange

            var inputJson = @"{
                ""FirstName"":""Arthur"",
                ""LastName"":""Bertrand"",
                ""Address"":{
                    ""StreetName"":""Gedempte Zalmhaven"",
                    ""Number"":""4K"",
                    ""City"":{
                        ""Name"":""Rotterdam"",
                        ""Country"":""The Netherlands""
                    },
                    ""ZipCode"":""3011 BT""
                },
                ""Age"":35,
                ""Hobbies"":[
                    ""Fishing"",
                    ""Rowing""
                ]
            }";
            var expectedResult = @"{""FirstName"":""Arthur"",""LastName"":""Bertrand"",""Age"":25,""Address"":{""StreetName"":""Gedempte Zalmhaven"",""Number"":""4K"",""ZipCode"":""3011 BT"",""City"":{""Name"":""Rotterdam"",""Country"":""The Netherlands""}},""Hobbies"":[""Fishing"",""Rowing""]}";

            using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(inputJson)))
            {
                // Act
                var outputStream = AttributeFirstJsonTransformer.Transform(inputStream); 

                // Assert
                using (var streamReader = new StreamReader(outputStream))
                {
                    var actualResult = streamReader.ReadToEnd();
                    Assert.AreNotEqual(expectedResult, actualResult);
                }
            }
        }

        [Test]
        public void WriteArray_WritesArrayWithAttributesFirst()
        {
            // Arrange
            var jsonWriterStrategy = new JsonWriterStrategy();
            var jsonArrayInput = JArray.Parse("[{\"name\": \"Harun\", \"friends\": [{\"name\": \"Test\"}]}, [1, 2, 3], \"test\"]");
            var expectedResult = "[{\"name\":\"Harun\",\"friends\":[{\"name\":\"Test\"}]},[1,2,3],\"test\"]";

            using (var stringWriter = new StringWriter())
            {
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    // Act
                    jsonWriterStrategy.WriteArray(jsonWriter, jsonArrayInput);

                    // Assert
                    var actualResult = stringWriter.ToString();
                    Assert.AreEqual(expectedResult, actualResult);
                }
            }
        }

        [Test]
        public void WhenEmptyArrayThenEmptyArray()
        {
            // Arrange
            var input = new MemoryStream(Encoding.ASCII.GetBytes("[]"));

            // Act
            var output = AttributeFirstJsonTransformer.Transform(input);
            var actual = new StreamReader(output).ReadToEnd();

            // Assert
            Assert.AreEqual("[]", actual);
        }

        [Test]
        public void WhenArrayWithObjectsThenAttributesFirstInObjects()
        {
            // Arrange
            var inputJson = @"[
                {
                    ""FirstName"": ""Harun"",
                    ""LastName"": ""Ugur"",
                    ""Age"": 30
                },
                {
                    ""FirstName"": ""Ugur"",
                    ""LastName"": ""Harun"",
                    ""Age"": 28
                }
            ]";
            var expectedOutputJson = @"[{""FirstName"":""Harun"",""LastName"":""Ugur"",""Age"":30},{""FirstName"":""Ugur"",""LastName"":""Harun"",""Age"":28}]";

            var input = new MemoryStream(Encoding.ASCII.GetBytes(inputJson));

            // Act
            var output = AttributeFirstJsonTransformer.Transform(input);
            var actual = new StreamReader(output).ReadToEnd();

            // Assert
            Assert.AreEqual(expectedOutputJson, actual);
        }

        [Test]
        public void WhenObjectWithOnlyAttributesThenNoChange()
        {
            // Arrange
            var inputJson = @"{
              ""FirstName"": ""Harun"",
              ""LastName"": ""Ugur"",
             ""Age"": 30
         }";
            var expectedOutputJson = @"{""FirstName"":""Harun"",""LastName"":""Ugur"",""Age"":30}";

            var input = new MemoryStream(Encoding.ASCII.GetBytes(inputJson));

            // Act
            var output = AttributeFirstJsonTransformer.Transform(input);
            var actual = new StreamReader(output).ReadToEnd();

            // Assert
            Assert.AreEqual(expectedOutputJson, actual);
        }

        [Test]
        public void WhenObjectWithMixOfAttributesNestedObjectsAndArraysThenAttributesFirst()
        {
            // Arrange
            var inputJson = @"{
              ""FirstName"": ""Harun"",
              ""LastName"": ""Ugur"",
              ""Age"": 30,
              ""Address"": {
              ""Street"": ""Main St"",
              ""City"": ""Los Angeles"",
              ""State"": ""CA""
            },
              ""Hobbies"": [
              ""Swimming"",
              ""Diving""
           ]
         }";
            var expectedOutputJson = @"{""FirstName"":""Harun"",""LastName"":""Ugur"",""Age"":30,""Address"":{""Street"":""Main St"",""City"":""Los Angeles"",""State"":""CA""},""Hobbies"":[""Swimming"",""Diving""]}";

            var input = new MemoryStream(Encoding.ASCII.GetBytes(inputJson));

            // Act
            var output = AttributeFirstJsonTransformer.Transform(input);
            var actual = new StreamReader(output).ReadToEnd();

            // Assert
            Assert.AreEqual(expectedOutputJson, actual);
        }

        [Test]
        public void WhenArrayWithMixOfValueTypesThenAttributesFirstInObjects()
        {
            // Arrange
            var inputJson = @"[
            82,
            {
               ""FirstName"": ""Harun"",
               ""LastName"": ""Test"",
               ""Age"": 10
            },
            ""Hello, world!"",
           [""X"", ""Y"", ""Z""]
           ]";
            var expectedOutputJson = @"[82,{""FirstName"":""Harun"",""LastName"":""Test"",""Age"":10},""Hello, world!"",[""X"",""Y"",""Z""]]";

            var input = new MemoryStream(Encoding.ASCII.GetBytes(inputJson));

            // Act
            var output = AttributeFirstJsonTransformer.Transform(input);
            var actual = new StreamReader(output).ReadToEnd();

            // Assert
            Assert.AreEqual(expectedOutputJson, actual);
        }

        [Test]
        public void WhenInvalidJsonInputThenThrowsInvalidOperationException()
        {
            // Arrange
            var inputJson = @"{
            ""FirstName"": ""Harun"",
            ""LastName"": ""Ugur"",
            ""Age"": 30, // Missing Closing Brace
            ";

            var input = new MemoryStream(Encoding.ASCII.GetBytes(inputJson));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => AttributeFirstJsonTransformer.Transform(input));
        }

        [Test]
        public void WhenEmptyInputStreamThenThrowsInvalidOperationException()
        {
            // Arrange
            var input = new MemoryStream();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => AttributeFirstJsonTransformer.Transform(input));
        }

        [Test]
        public void WhenNullInputThenThrowsArgumentNullException()
        {
            // Arrange
            Stream input = null;
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => AttributeFirstJsonTransformer.Transform(input));
        }

        [Test]
        public void WhenNestedObjectsAndArraysWithAttributesThenAttributesFirst()
        {
            // Arrange
            var inputJson = @"{
            ""FirstName"": ""Harun"",
            ""LastName"": ""Ugur"",
            ""Contact"": {
            ""Email"": ""harun.ugur@example.com"",
            ""Phone"": {
                ""Home"": ""222-1234"",
                ""Work"": ""222-5678""
            }
          },
            ""Skills"": [
            {
                ""Name"": ""CSharp"",
                ""Level"": ""Expert""
            },
            {
                ""Name"": ""React"",
                ""Level"": ""Intermediate""
            }
           ]
          }";
            var expectedOutputJson = @"{""FirstName"":""Harun"",""LastName"":""Ugur"",""Contact"":{""Email"":""harun.ugur@example.com"",""Phone"":{""Home"":""222-1234"",""Work"":""222-5678""}},""Skills"":[{""Name"":""CSharp"",""Level"":""Expert""},{""Name"":""React"",""Level"":""Intermediate""}]}";
            var input = new MemoryStream(Encoding.ASCII.GetBytes(inputJson));

            // Act
            var output = AttributeFirstJsonTransformer.Transform(input);
            var actual = new StreamReader(output).ReadToEnd();

            // Assert
            Assert.AreEqual(expectedOutputJson, actual);
        }

        [Test]
        public void WhenDeeplyNestedObjectsThenAttributesFirst()
        {
            // Arrange
            var inputJson = @"{
         ""A"": {
             ""B"": {
                ""C"": {
                  ""D"": {
                    ""E"": {
                        ""F"": {
                            ""G"": {
                                ""H"": ""Deeply Nested""
                            },
                            ""X"": 83
                            }
                        }
                     }
                   }
                }
              }
            }";
            var expectedOutputJson = @"{""A"":{""B"":{""C"":{""D"":{""E"":{""F"":{""X"":83,""G"":{""H"":""Deeply Nested""}}}}}}}}";

            var input = new MemoryStream(Encoding.ASCII.GetBytes(inputJson));

            // Act
            var output = AttributeFirstJsonTransformer.Transform(input);
            var actual = new StreamReader(output).ReadToEnd();

            // Assert
            Assert.AreEqual(expectedOutputJson, actual);
        }
    }
}