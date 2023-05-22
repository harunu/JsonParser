# JsonParser
JsonParser
JSON Attribute First Transformer
This repository contains a solution for transforming JSON content by reordering properties to ensure attributes are written first, followed by nested objects and arrays. This solution reads and parses the JSON from an input stream, transforms the structure according to the specified order, then writes the transformed JSON to an output stream.

SOLID principles and Design Patterns have been implemented to ensure maintainability and testability of the solution.

Design Patterns and Principles
The following design patterns and principles are applied in this solution:

Strategy Pattern
The IJsonWriterStrategy interface and its concrete implementation JsonWriterStrategy follow the strategy pattern, which encapsulates each algorithm and makes them interchangeable. JsonWriterStrategy class defines a specific algorithm for writing JSON objects and arrays in the desired order.

Factory Method Pattern
The static Transform method in the AttributeFirstJsonTransformer class acts as a factory method. It creates an instance of AttributeFirstJsonTransformerImpl with the appropriate JsonWriterStrategy and calls the Transform method on that instance. This pattern can be useful for encapsulating object creation and providing a consistent interface for creating objects.

Interface Segregation Principle (ISP)
The IJsonWriterStrategy interface is small, and contains only methods related to writing JSON objects and arrays. This principle ensures that interfaces remain lean and focused on a specific purpose, making them easier to implement and understand.

Dependency Inversion Principle (DIP)
The AttributeFirstJsonTransformerImpl class depends on the abstraction IJsonWriterStrategy instead of a concrete implementation. This principle helps to create more flexible and maintainable code by reducing the coupling between classes.

Single Responsibility Principle (SRP)
The different classes and methods in this implementation have clear and specific responsibilities. For example, the AttributeFirstJsonTransformerImpl class is responsible for transforming the JSON structure, while the JsonWriterStrategy class handles the writing of JSON objects and arrays.

Usage
Install the necessary dependencies and build the project. In order to handle Dependency Injection, libraries like Autofac, Moq, or others may be needed.

A ContainerBuilder should be created and the dependencies registered appropriately. Note that the original code design has not been altered for the purpose of Dependency Injection.

Test-Driven Development (TDD)
The TDD approach has been implemented. A test was written and test cases were run against the implementation. The AttributeFirstJsonTransformer class, which contains a Transform method, was developed to pass the test cases. This method takes a JSON input stream and returns a JSON output stream with attributes reordered to appear before nested objects and arrays.

With the successful implementation of the above, the code was refactored to enhance its design, readability, and performance while keeping the test cases passing.

If new edge cases or requirements emerge, additional test cases can be added and the process can be repeated.

Contributing
Contributions, issues and feature requests are welcome! Feel free to check issues page.

License
MIT

Show your support
Give a ⭐️ if this project helped you!
