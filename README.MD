Open appsettings.json and make any changes to the parameters that you want:

Parameters:
Interval: How often Busy Bot wakes up "HH:MM:SS"
FindClass: Window class name to look for, Default is Notepad
Message: Message that Busy Bot types
LinesToWrite: Number of lines to write until the next set

Now just open Notepad or whatever program you want that matches the windows class and run it.

There is a button on the window that says Start. Press that and Busy Bot will start counting down and then sending text to the target class window over and over again. 
Pressing the same button now labeled Stop causes Busy Bot to stop sending text messages.

If BusyBot can't find the window then it enters a faulted state. The program must be stopped and the configuration checked and changed and then it can be restarted again.

Note: When Busy Bot is typing if you click somewhere else that will change the focused window but the SendKeys command WILL finish so the results are unpredictable.
