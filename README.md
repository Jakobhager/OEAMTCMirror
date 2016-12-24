# OEAMTCMirror    
[Last compiled version](https://github.com/inrebinfo/OEAMTCMirror/blob/master/bin/Debug/OEAMTCMirror.exe)    

##Registry Keys
Zu finden unter: HKEY_CURRENT_USER\\\SOFTWARE\\\OEAMTCMirror bzw. HKEY_LOCAL_MACHINE\\\SOFTWARE\\\WOW6432Node\\\OEAMTCMirror
 - Initiated
	 - Wert wird beim ersten Start erstellt und auf true gesetzt
 - ScreenshotWindows
	 - Wenn gesetzt, werden nur die Prozessnamen welche angeben sind gescreenshottet, der Rest mittels PrintWindow
 - ExcludedWindows
	 - In diesen Fenstern wird der Button in der Titlebar ausgeblendet
 - Hotkey
	 - Der Hotkey zum Start der Spiegelung (muss wie in der Liste hier https://msdn.microsoft.com/en-us/library/system.windows.forms.keys(v=vs.110).aspx angegeben werden)
 - MirrorIndex
	 - Der Index des Screens auf den gespiegelt werden soll, es wird mit 1 begonnen. 2 Bildschirme, Spiegelung auf Bildschirm 2 -> Value auf 2