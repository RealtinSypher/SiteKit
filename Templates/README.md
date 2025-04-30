# SiteKit Templates

## Developing Custom Templates

Begin by using the default structure as your foundation for creating custom templates.

**Folder Organization:**
```bash
myTemplate/
├─ Layout/   # Contains MainLayout.html.
├─ Pages/    # Stores Markdown or HTML content.
├─ wwwroot/  # Houses CSS, images, JavaScript files, etc.
```

After assembling your custom folder, compress it into a `.zip` file (for example, myTemplate.zip). Make sure that the zip archive’s root contains the folder’s inner files directly following the structure outlined above.

Once zipped, transfer the file to the `UserDirectory/.SiteKit/templates` directory.

You can then generate new projects using your custom template:
```bash
sitekit new myBlog --template myTemplate
```