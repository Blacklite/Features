# Features for Asp.Net 5

### What are features?
Features are an idea based on the idea of adding progressive feature to your application.  As referenced by [Martin Fowler][1].  They are a way to introduce a new feature to your application without breaking production at the same time.

### What does this library do for you?
Features handles a few things for you, so you don't have to.  Features spans the concepts of `release toggles` and `business toggles`, allowing you to focus on adding value to your software without a lot of worring about configuration of some specific feature.

* New Features
Using a few simple base classes, you can add a feature in minutes to your application.  You can make these features off by default when you first check them in, it won't break production.
* Creating feature toggles
Features can be true feature toggles where the value can be configured on or off.
* Business Settings
Features can have additional properties.  They allow you to have artibrary information stored along side of your feature.  For example you might be adding support for texting your users, your new feature might be a `Switch` but it wodul also have your Twilio API key along with it.
* Observable
Sometimes features are a core piece of your application, but turning off a feature shouldn't mean that your application has to restart for the change to be picked up.  Features includes the ability to have long lived `Observable` features.  This way you can make changes at runtime and have them be reflected almost immediately.
* Dependencies
In some cases one feature depends on another, or one feature is mutually exclusive another.  For example you might have two different services for texting your tenants, and you won't want both to be on at the same time, but you want to give your users the choice.

[1]: http://martinfowler.com/bliki/FeatureToggle.html "Feature Toggles"
