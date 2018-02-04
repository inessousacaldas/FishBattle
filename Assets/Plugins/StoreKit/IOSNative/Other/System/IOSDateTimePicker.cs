using UnityEngine;
using System;
using System.Collections;


#if IOSNATIVE_ENABLED && (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif




public class IOSDateTimePicker : ISN_Singleton<IOSDateTimePicker>  {

	#if IOSNATIVE_ENABLED && (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	
	[DllImport ("__Internal")]
	private static extern void _ISN_ShowDP(int mode);
		
	#endif

	public Action<DateTime> OnDateChanged = delegate {};
	public Action<DateTime> OnPickerClosed = delegate {};

	


	//--------------------------------------
	// Public Methods
	//--------------------------------------
		
		
	public void Show(IOSDateTimePickerMode mode) {
		#if IOSNATIVE_ENABLED && (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_ShowDP( (int) mode);
		#endif
	}



	//--------------------------------------
	// Events
	//--------------------------------------

	private void DateChangedEvent(string time) {
		DateTime dt  = DateTime.Parse(time);

		OnDateChanged(dt);
	}

	private void PickerClosed(string time) {
		DateTime dt  = DateTime.Parse(time);

		OnPickerClosed(dt);
	}
}
