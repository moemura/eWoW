This application is simple. But that's a good thing!

It doesn't do anything fancy, nor does it have a whole ton of options.

It's only job is to keep the servers running (if you choose to), and hide the windows (if you choose to)

It will display the server component status in the tray icon tooltip.

If you want to view debug messages, and the normal emu output stuff, simply uncheck "Hide Windows"
	and you'll be able to see them.
	
Please note: Since for whatever reason, the Process.Exited event seems to fire at very random times,
	I've implemented the running check in a timer. This fixes the issue, but does incur some performance
	issues. (You won't notice it at all, unless you're running a VERY old system.)
	
	It will check if the processes are running once per second. Eventually, if we decide to get real
	elaborate, we can add a whole ton of settings and whatnot. (If requested, I can even create an entire
	setup process to make things super easy.)