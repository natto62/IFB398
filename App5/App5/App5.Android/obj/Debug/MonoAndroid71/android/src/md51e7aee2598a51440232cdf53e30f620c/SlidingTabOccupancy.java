package md51e7aee2598a51440232cdf53e30f620c;


public class SlidingTabOccupancy
	extends android.support.v4.app.FragmentActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("MiCareApp.Droid.SlidingTabOccupancy, MiCareApp.Android", SlidingTabOccupancy.class, __md_methods);
	}


	public SlidingTabOccupancy ()
	{
		super ();
		if (getClass () == SlidingTabOccupancy.class)
			mono.android.TypeManager.Activate ("MiCareApp.Droid.SlidingTabOccupancy, MiCareApp.Android", "", this, new java.lang.Object[] {  });
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
