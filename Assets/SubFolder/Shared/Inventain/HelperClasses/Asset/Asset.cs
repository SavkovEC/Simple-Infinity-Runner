using UnityEngine;


public abstract class Asset
{
	public virtual bool isLoaded
	{
		get
		{
			return true;
		}
	}


	public abstract Object asset
	{
		get;
	}


	public virtual System.Type type
	{
		get
		{
			return typeof(Object);
		}
	}


	public virtual Object Load(){ return null; }
	public virtual void LoadAsync(){}
	public virtual void Unload(){}


	public virtual T Pick<T>(bool unload = true) where T : Object
	{
		T result = this.asset as T; 
		if(unload)
		{
			Unload();
		}
		return result;
	}


	public virtual Object GetInstance()
	{
		return Object.Instantiate(asset);
	}


    public virtual Asset Copy()
    {
        return this;
    }
}