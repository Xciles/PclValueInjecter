PclValueInjecter
================
Currently this is still a 'testing' version and will be improved ;). After more testing I will create and publish a Nuget package.


This project provides a Portable Class Library implementation of Omu's [ValueInjecter](http://valueinjecter.codeplex.com/).

The main purpose of this project is to make ValueInjecter also available for Mono projects like Xamarin.Android & Xamarin.iOS.

PclValueInjecter also includes the Automapper simulation logic which can be found [here] (http://valueinjecter.codeplex.com/wikipage?title=Automapper%20Simulation&referringTitle=Home).


Documentation and Examples
==========================
(Code examples are from http://valueinjecter.codeplex.com/)
#### Basic usage
``` C#
myObject.InjectFrom(anyOtherObject);

// inject from multiple sources
a.InjectFrom(b,c,d,e);

// inject using your own injection
a.InjectFrom<MyInjection>(b);
```

#### Automapper Simulation
```
Mapper.Map(foo, bar) // will map from foo to an existing bar

var bar = Mapper.Map<Foo,Bar>(foo) // will create a new bar, map foo to it and return it

MapperFactory.AddMapper(new FooBar()) // equivalent to Automapper's CreateMap<Foo,Bar>, 
// except here you do this only if you need to specify something that doesn't fit into the default conventions

// FooBar is a class that implements ITypeMapper<TSource,TTarget> 
// (inherit the base class TypeMapper<TSource, TTarget> and add additional mapping code)
```

Licensing
=========
This project is developed and distributed under MIT License
* ValueInjecter - MIT License see http://valueinjecter.codeplex.com/license

References
==========
[ValueInjecter](http://valueinjecter.codeplex.com/)

[Automapper Simulation] (http://valueinjecter.codeplex.com/wikipage?title=Automapper%20Simulation&referringTitle=Home)
