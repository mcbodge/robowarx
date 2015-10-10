# Introduction #

For those of you eager to jump in, here's some short steps to getting up and running with the RoboWarX source code!

Some knowledge of C# and Subversion is probably helpful, though not required.


# Getting the code #

The source code is stored here on Google Code in a Subversion repository.

## Command-line ##

Take a look at the [“Source → Checkout” page](http://code.google.com/p/robowarx/source/checkout), which shows you an example command-line for getting the latest source code. Usually, for non project members, this is something like:

```
# svn checkout http://robowarx.googlecode.com/svn/trunk/robowarx/
```

Keeping your source code up to date after the initial checkout is easy. Simply open a command-line and update as follows:

```
# cd robowarx
# svn update
```

## MonoDevelop 2.0 ##

In MonoDevelop 2.0, you can also use the “File” → “Checkout” option. In this dialog, switch to the repositories tab and add a new repository. You'll need the following info (for non project members):
  * Protocol: http
  * Server: robowarx.googlecode.com
  * Path: /svn
  * Optionally fill in the name field.
  * Leave the other fields as is.

After clicking OK, click the arrow to expand your new repository, until you can select “trunk” → “robowarx”. You probably want to point the target directory field at an empty directory somewhere, then click OK.

To keep your code up to date, right click the root node in the solutions sidepane, which should be labeled “Solution RoboWarX”, and select “Version Control” → “Update”.

## Visual Studio 2008 ##

TO BE WRITTEN: there's a subversion plugin for Visual Studio 2008 (non-Express) called AnkhSVN.


# Source code layout #

At the time of writing, the complete source is built out of three projects:
  * LibRoboWarX, which contains all of the game logic.
  * RoboWarX.GTK, which contains the GTK+ graphical frontend.
  * RoboWarX.Winforms, which contains the Windows graphical frontend.

If you're a MonoDevelop 2.0 or Visual Studio 2008 user, there are solution files next to these directories which you use.

Additionally, there's a tests directory which currently contains some simple test robots.


# Building and running #

Building the source is fairly straight forward. Unless we broke the current code, you shouldn't encounter many difficulties.

## Command-line ##

For Unix systems, there is a way to build from the command-line. However, you still need MonoDevelop 2.0 installed to perform a build. Once installed, move into the root of the source directory, where the RoboWarX.mds solution file is located, and execute:

```
# mdtool build
```

Once built, each of the projects will have a “bin” directory. Inside this directory is another directory named after the build configuration, usually “Debug” or “Release”. These contain the actual binaries. To execute the GTK+ frontend, for example:

```
# RoboWarX.GTK/bin/Debug/RoboWarX.GTK.exe
```

TO BE WRITTEN: Windows systems with Visual Studio 2008 installed can do this as well. But how?

## MonoDevelop 2.0 / Visual Studio 2008 ##

Hit the “Build solution” or “Run” button and you should be set.


# Making changes #

There's not a lot of magic to making changes with Subversion. Here's a small intro, but remember that there's [far more extensive documentation](http://svnbook.red-bean.com/) on Subversion out there on the internet.

Remember that you can only upload changes if you are a project member, and the directory you are working in was checked out using your Google Code account over HTTPS.

## Command-line ##

You start out by simply making your changes, adding and removing files or directories, etc. Then “commit” them all to the source code repository in one batch.

It's good practice to keep commits concise, as in: don't create 20 new features or fix many bugs in one commit. Keeping it down to single changes per commit helps in:
  * keeping the revision log clean,
  * reviewing code changes,
  * tracing back regressions to a single change.

Therefor, after you've made your changes (and tested them), it's never a bad idea to double check what you're about to commit. The most straight forward way to do this is:

```
# svn status
```

This will show you a list of altered files, prefixed with:
  * M for modified
  * D for deleted
  * ? for files not currently tracked in version control.
  * (there are more statuses not described here.)

You'll notice any new files will show up as untracked. You'll have to add them using:

```
# svn add <files>
```

For more detailed output of your changes, you can see them all in standard diff format. You probably want to view this output in a pager or write it to a file. For example:

```
# svn diff | less
```

Finally, when you're all set, you can commit using:

```
# svn commit
```

You'll be greeted by a text editor so you can enter a commit message. Write one, save, exit and that's that.

Alternatively, commit specific files using:

```
# svn commit <files>
```

## MonoDevelop 2.0 ##

TO BE WRITTEN

## Visual Studio 2008 ##

TO BE WRITTEN