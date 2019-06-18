using System;
using System.Collections.Generic;
using System.Text;

public class ZComponent
{
	public ZObject owner;

	public virtual void init(Object param) { }
}
public class ZObject
{
	Dictionary<Type,ZComponent> dicComponent;
	public ZObject()
	{
		dicComponent = new Dictionary<Type,ZComponent>();
	}
	public T AddComponent<T>(Object param) where T : ZComponent
	{
		T t = (T)Activator.CreateInstance(typeof(T));
		t.owner = this;
		t.init(param);
		dicComponent.Add(typeof(T), t);
		return t;
	}
	public T GetComponent<T>() where T : ZComponent
	{
		ZComponent t;
		if (dicComponent.TryGetValue(typeof(T), out t))
			return (T)t;
		return null;
	}
}

