ImgurDotNet
===========

A .NET wrapper for the Imgur API, Version 3

In order to use these APIs, you must have a Client ID, which you will recieve when you register your appliation with Imgur: https://api.imgur.com/oauth2/addclient

Currently, this library only has support for accessing public read-only resources and anonymous actions. In other words, account authentication is not yet supported. With this library, you will be able to:  
 - Get information about an existing album
 - Get information about an existing image
 - Get information about an account
 - Get information about a comment
 - Create an album
 - Add image created album upon image upload
 - Upload an image from the web using a direct link
 - Upload an image file using a file path, binary file, or a System.Drawing.Image object.
 - Delete an image
 - Delete an album

<b>Coming soon:</b>
 - Get information about the many other Imgur data models
 - Account authentication

License
===========

ImgurDotNet
Copyright (C) 2012  Jaco Ruit

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
