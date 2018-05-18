package md574177612645b31f0ca9f83df1c638cfa;


public class ItemPage
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("MiCareApp.Droid.ItemPage, App5.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ItemPage.class, __md_methods);
	}


	public ItemPage ()
	{
		super ();
		if (getClass () == ItemPage.class)
			mono.android.TypeManager.Activate ("MiCareApp.Droid.ItemPage, App5.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
