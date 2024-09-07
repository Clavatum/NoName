using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Container class that wraps a value and ensures initialisation is 
/// called just before first use.
public class LazyValue<T>
{
    private T _value;
    private bool _initialized = false;
    private InitializerDelegate _initializer;

    public delegate T InitializerDelegate();

    /// Setup the container but don't initialise the value yet.
    /// <param name="initializer"> 
    /// The initialiser delegate to call when first used. 
    public LazyValue(InitializerDelegate initializer)
    {
        _initializer = initializer;
    }

    /// Note that setting the value before initialisation will initialise 
    /// the class.
    public T value
    {
        get
        {
            // Ensure we init before returning a value.
            ForceInit();
            return _value;
        }
        set
        {
            // Don't use default init anymore.
            _initialized = true;
            _value = value;
        }
    }

    /// Force the initialisation of the value via the delegate.

    public void ForceInit()
    {
        if (!_initialized)
        {
            _value = _initializer();
            _initialized = true;
        }
    }
}
