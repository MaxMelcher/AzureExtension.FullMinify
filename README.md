# Azure Site Extension: FullMinifier

This site extension minifies html, css, js and potentially (**not yet!**) images.

On startup of the webapp, the default directory (**D:\home\site\wwwroot\\**) is scanned for **.js**, **.html**, **.css** files. The files are then compressed and replaced.
If a file is changed during runtime, they will be reprocessed immediately.

This project was greatly inspired by the existing site extension [Azure Image Optimizer](https://www.siteextensions.net/packages/AzureImageOptimizer/) by [Sayed-Ibrahim-Hashimi](https://twitter.com/sayedihashimi) and [Mads Kristensen](https://twitter.com/mkristensen) and is based on the following libaries:

## Html Minifier

[Zeta Producer Html Compressor](https://github.com/UweKeim/ZetaProducerHtmlCompressor), version 1.0.2

>A .NET port of Google’s HtmlCompressor library to minify HTML source code.

## CSS Minifier

[Efficient stylesheet minification in C#](https://madskristensen.net/blog/efficient-stylesheet-minification-in-c)

>The method takes a string of CSS and returns a minified version of it. The method have been modified for demo purposes, so you might want to optimize the code yourself.

## JS Minifier

[JSMin.Net](https://github.com/Taritsyn/JSMin.NET), version 1.1.3

> JSMin.NET is a .NET port of the [Douglas Crockford's JSMin](http://github.com/douglascrockford/JSMin).

## Image Minifier

The image minifier is based on a couple of tools that Mads Kristensen picked:

- gifsicle.exe to minify gifs
- jpegtran.exe to minify jpgs
- zopflipng.exe to minify pngs

> Currently the images are lossy compressed to get the best results. A configuration flag will follow.

## Setup

**tbd**

## Configuration

The following can be configured:

### The directory that will be scanned for files: **minify.path**

Default: D:\home\site\wwwroot\
Overwrite them in the app setting of the website.

### The file extensions of the files that will be minified: **minify.extensions**

Default: .css;.html;.js
Overwrite them in the app setting of the website.