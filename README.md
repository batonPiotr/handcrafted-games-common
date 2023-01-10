# Common

Common library used by other HG Tools.

## Installation

## UPM

Add package from git URL `https://github.com/batonPiotr/handcrafted-games-common.git?path=Assets/Scripts`

You have to manually add following dependencies:

- Add package from git URL `https://github.com/neuecc/UniRx.git?path=Assets/Plugins/UniRx/Scripts`. Since UPM doesn't support git dependencies, it has to be installed manually.
- Add package by name: `com.unity.test-framework.performance`. Since Unity doesn't allow experimental dependencies, you have to install it manually.

## Usage

### Dependency Injection

 You can register and resolve services in various scopes:
 - In a game object. Useful when you create complex behaviours and don't want to fight with complex components structure
 - In a scene. You can register components/classes in scene scope. They will be removed once the scene is unloaded.
 - Global. This DI Container will stay as long as the application is running. Useful for creating global services for handling app lifecycle or spawning further services

#### Preparations
To enable working with DI, you have to add following tags in Unity Editor:
`SceneDIContainer` and `GlobalDIContainer`.

#### Registering
To register a service, you have to register its instance in a `Awake` method in a component of your chosing. You then select your DI scope as follows:
- Game object:
```
var DI = gameObject.GetGODependencyInjection();
```
- Scene:
```
var DI = gameObject.scene.GetGODependencyInjection(true); // Set to true if you want to create the Scene DI automatically if not created yet.
```
- Global:
```
var DI = gameObject.GetGlobalDependencyInjection();
```

You can then register the instance:
```
DI.Register(someInstance);
```

#### Defining dependencies

If you want to use a dependency in a component or class, you define it as follows:
```
[Inject] private YourClassType dependency;
```

#### Resolving

Since the lifecycle of objects is not defined or is managed by Unity itself, you have to trigger the resolving process manually, by calling:
```
DI.ResolveDependencies(this);
```

You can optionally get a callback when dependencies are resolved by defining some void method without parameters with `[OnDependenciesDidResolve]` attribute.

#### Defining resolving strategy

Services will be discovered automatically, whether they need to be resolved from the context of current GameObject, scene or global. But sometimes you want to be able to specify the source. Especially when there is multiple instances of one type. To do this, you simply put the strategy in the `[Inject]` Attribute like this:
`[Inject(ResolveSource.Parent)]`

All resolving strategies:
- Self: It searches only in the DI context of the current Game Object.
- ExplicitDependencies: You can specify other game objects your current one depends upon. You have to add these instances with `DI.AddDependency(otherGameObject)` to be able to search dependencies within the context of the other Game Object.
- Children: It searches in its children.
- Parent: It searches only in its direct parent.
- Parents: It searches in every parent up until the root object.
- Scene: It searches in the scene DI context.
- Global: It searches in the global DI context.

You can combine multiple sources by specifying them in the `Inject` attribute: `[Inject(ResolveSource.Parent | ResolveSource.Self)]`.