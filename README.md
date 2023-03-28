# ASP.NET Core 6 MVC - Creating a view with a controller

So far we have looked at:

- Creating an API with full CRUD functionality
- Review of what makes up an MVC app

- Intro to authentication and authorisation

In this lesson, we are going to create a page in our app which returns a list of To Do Items

(Note: in the next lesson, include seed data)

## Walkthrough

### Create the ToDo item Model 

In the "Data" Folder, add a new folder called "Models". In models create a new file called `ToDo.cs` and add the following code:

```csharp
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SpartaToDo.Data.Models
{
    public class ToDo
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        [Display(Name = "Complete ?")]
        public bool Complete { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Created")]
        public DateTime DateCreated { get; init;} = DateTime.Now;
    }
}

```

Rename `ApplicationContext` to `SpartaToDoContext` then add `public DbSet<ToDo> ToDos { get; set; }` to the class (**rename all**):

```csharp
public class SpartaToDoContext : IdentityDbContext
{
    public SpartaToDoContext(DbContextOptions<SpartaToDoContext> options)
        : base(options)
    {
 
    }

    public DbSet<ToDo> ToDos { get; set; }
}
```

Change the connection string in `appsettings.json` to:

```c#
"Server=(localdb)\\mssqllocaldb;Database=aspnet-SpartaToDo;Trusted_Connection=True;MultipleActiveResultSets=true"
```



Now in the PMC:

```
add-migration [name of migration]
update-database
```

### Seed data

First thing we're going to do is seed the database. In "Data", create a new class called `SeedData`:

```csharp
 public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var db = new SpartaToDoContext(
              serviceProvider.GetRequiredService<
                  DbContextOptions<SpartaToDoContext>>()))
            {
                // Look for any ToDos.
                if (db.ToDos.Any())
                {
                    return;   // DB has been seeded
                }

                db.ToDos.AddRange(

                    new ToDo
                    {
                        Title = "Teach C#",
                        Description = "Teach Engineering-2022 how to use Entity Framework",
                        Complete = true,
                        DateCreated = new DateTime(2022, 03, 01)
                    },
                    new ToDo
                    {
                        Title = "Learn two swim",
                        Description = "Dive off from the Cliffs of Dover and swim until I get to France",
                        Complete = false
                    },

                    new ToDo
                    {
                        Title = "Reunite Oasis",
                        Description = "Need to arrange chat with Noel and Liam to sort out a reunion tour",
                        Complete = false
                    });
                db.SaveChanges();
            }
        }
    }
```

Now we need to add a scoped service. In the `Program.cs` file add the using statement

```csharp
using SpartaToDo.Data;

///code

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}
```

`scope` is of `IServiceScope` type - one instance per request to the application will be created. and will be disposed of after leaving the `using`block - we call `SeedData.Initialize()` method. (needs more details and rewording)

Run the program, then close it. Look at the "ToDos" table in the "SpartaToDo" database - our seeded data has been added!

## Adding a controller with views

Right click on the "Controllers" folder, then click "Add New Scaffolded Item". Add "MVC Controller with views using Entity Framework".

- Model class: `ToDo (SpartaToDo.Data.Models)`
- Data Context class: `SpartaToDoContext (SpartaToDo.Data)`
  - [x] Generate views
  - [x] Reference script Libraries
  - [x] Use a layout page
- Controller name: `ToDoController`

Examine the code generated - we have created a model and a views(`Views >> ToDo`), a controller(`ToDoController.cs`) and model view(`ToDoViewModel.cs`).

### Views 

First let's look at our views, we have 5 of them! If we run our program and navigate to `https://localhost:[port number]/todo`, we see that all our "ToDos" are present! When we press delete on the first ToDo item, we are taken to the page `ToDo/Delete/1`. The order of or directories and files, and their names, correspond to the URI address. 

But what I don't want the "Delete page" - I want to delete from the main screen. Also, when I view "Details", the box for description is a bit smaller than I like. Let's deal with those. But first let's make it so that the homepage contains a link to our to do list.

> **TRAINER PROMPT**
>
> Put a break point in `Index`. This is loaded first. Ask class why?
>
> The Default route includes defaults for all three parameters. If you don't supply a controller, then the controller parameter defaults to the value **Home**. If you don't supply an action, the action parameter defaults to the value **Index**. Finally, if you don't supply an id, the id parameter defaults to an empty string. https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/controllers-and-routing/asp-net-mvc-routing-overview-cs
>
> Put a breakpoint in edit and then edit an item in the view

#### Update Nav Bar to link to To Do page

**UPDATE-DATABASE and PUT BREAKPOINT IN**

##### Using Tag Helpers to call the controller and models

Now firstly, let's create a link to our `Layout.cshtml`page. Under line 27 in, let's add another list item:

```html
<li class="nav-item">
    <a class="nav-link text-dark" asp-area="" asp-controller="ToDo" asp-action="Index">To Do List</a>
</li>
```

The HTML generate from this is:

```html
<li b-5wu0ndmrzw="" class="nav-item">
    <a class="nav-link text-dark" href="/ToDo">To Do List</a>
</li>
```

We will come back to our `ToDoController` method `Index` later on.

Now, navigate to `Views >> ToDo >> Index` . The HTML, when rendered will make a table and populate the it's headers and data by calling methods from our `ToDo` model. Notice the Razor syntax at the top of the page:

```html
@model IEnumerable<SpartaToDo.Data.Models.ToDo>
```

We are using the our an `IEnumerable` of `ToDo` items. If we look in our `ToDoController` , we see see the `Index` method returns a `View`, but the `View` method takes a list of `ToDoItems`, which is assigned to our model in our razor page. I.e. when the page is rendered, this:

```csharp
@model IEnumerable<SpartaToDo.Data.Models.ToDo>
```

equals this 

```c#
_context.ToDos.ToListAsync()
```

model to get the display name for the `ToDo` model's properties. E.g.@

```csharp
<th>
    @Html.DisplayNameFor(model => model.DateCreated)
</th>
```

In the example above, we are just setting the display name as the `ToDo` property name the display name we gave `DateCreated` which was `Created`

When it populates the table data as shown below:

```csharp
//For every item in our list, generate a new row of data
@foreach (var item in Model) {
        <tr>
            <td>
            //Which invlude the titele for that item
                @Html.DisplayFor(modelItem => item.Title)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Description)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Complete)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.DateCreated)
            </td>
            <td>
                <a asp-action="Edit" asp-route-id="@item.Id">Edit</a> |
                <a asp-action="Details" asp-route-id="@item.Id">Details</a> |
                <a asp-action="Delete" asp-route-id="@item.Id">Delete</a>
            </td>
        </tr>
}
```

We're iterating through each item in our `ToDo` model and setting the table data to match each what we "get" from each property. What smart about this, it even recognises the type and infers the type of element to display (e.g. `Completed` is a bool, and when our application runs, a checkbox is rendered).

This is what we call **[MODEL BINDING](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-6.0)**

We can have one model per page. But what if I want to display other things, such as user information? We can create our own ViewModel and use that! We will cover it tomorrow when we extend our Identity class.

##### Tag helpers overview

Notice the tag helpers. If we look at the `input` tag helper, based on the data type, it sets the HTML type attribute accordingly. 

> Below is taken from from the [MSDN](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms?view=aspnetcore-6.0):
>
> The `Input` Tag Helper sets the HTML `type` attribute based on the .NET type. The following table lists some common .NET types and generated HTML type (not every .NET type is listed).
>
> | .NET type      | Input Type                                                   |
> | :------------- | :----------------------------------------------------------- |
> | Bool           | type="checkbox"                                              |
> | String         | type="text"                                                  |
> | DateTime       | type=["datetime-local"](https://developer.mozilla.org/docs/Web/HTML/Element/input/datetime-local) |
> | Byte           | type="number"                                                |
> | Int            | type="number"                                                |
> | Single, Double | type="number"                                                |

What do the other attributes mean?:

- `asp-area`: Sets the area name used to set the appropriate route. The following examples depict how the `asp-area` attribute causes a remapping of routes. Navigate to the "Login page" here they use controllers
- `asp-controller`: Assigns the controller used for generating the URL. In this case it will know the controller is "HomeController".  Anchor tag helper.
- `asp-action `: The method we are calling from the "HomeController" - in this case it will be `Index()`, which returns `View()` (i.e. home/index which is the what `View()` returns - the page associated to that controller). Anchor tag helper.

There are more tag helpers in our project. Go to the `Create.cshtml` file and point out the following:

- `asp-for`:
  - if the HTML attribute is a label, it will get the property name e.g. `<label asp-for="Description"...` 
  - If the HTML attribute is an input field, it will extract the name of the specified model property into the rendered HTML e.g. `<input asp-for="Description"...`
- `asp-validation-for`: It adds the *data-valmsg-for="property name"* attribute to the element which it carries for example span. It attaches the validation message on the input field of the specified Model property. The client-side validation can be done with jQuery. It is an alternative to `Html.ValidationMessageFor`
- `asp-validation-summary`: It displays a summary of validation error messages. It targets "DIV" element with "asp-validation-summary" attribute. Look at the login page again.
- `asp-route-{value}`: Used for creating a URL linking directly to a named route. Any value occupying the `{value}` placeholder is interpreted as a potential route parameter. 

There are quite a few others - we do not have time to go through them all, but more information can be found [here](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/built-in/anchor-tag-helper?view=aspnetcore-6.0)



#### Change Register and Login buttons (OPTIONAL)

Note that in the `_Layout.cshtml` file,  we have the line` <partial name="_LoginPartial" />` -  this will render the `_LoginPartial.cshtml` in the header. `partial` means partial class `name` is the name of that partial class we are being directed to.

Whilst we're here, let's make the register and login buttons look nicer using our bootstrap scripts. On line 19 onwards, change the class of the anchor tag so it includes the extra information shown below:

```html
<li class="nav-item" style="margin:5px;">
    <a class="nav-link btn btn-primary text-light" asp-area="Identity" asp-page="/Account/Register">Register</a>
</li>
<li class="nav-item" style="margin:5px;">
    <a class="nav-link btn btn-outline-primary" asp-area="Identity" asp-page="/Account/Login">Login</a>
</li>
```

#### Make description boxes bigger (OPTIONAL)

We can add update our `Create.cshtml` from...

```html
<input asp-for="Description" class="form-control" />
```

*to*

```html
<textarea asp-for="Description" class="form-control rounded-0" rows = "10"></textarea>
```

We can add update our `Edit.cshtml` from...

from

```html
<input asp-for="Description" class="form-control" />
```

*to*

```c#
<textarea asp-for="Description" class="form-control rounded-0" rows = "10"></textarea>
```



### Controller overview

Before we talk about the controller, let's talk about model bidning

> Taken from [MSDN](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-6.0):
>
> Controllers and Razor pages work with data that comes from HTTP requests. For example, route data may provide a record key, and posted form fields may provide values for the properties of the model. Writing code to retrieve each of these values and convert them from strings to .NET types would be tedious and error-prone. Model binding automates this process. The model binding system:
>
> - Retrieves data from various sources such as route data, form fields, and query strings.
> - Provides the data to controllers and Razor pages in method parameters and public properties.
> - Converts string data to .NET types.
> - Updates properties of complex types.

Now let's look at the Controller we created - it looks similar to our controllers when we created an API, but a bit more complicated.

##### Constructor

First let look at the constructor:

It takes our `SpartaToDoContext` as a parameter:

```csharp
private readonly SpartaToDoContext _context;

public ToDoController(SpartaToDoContext context)
{
    _context = context;
}
```

We are able to use this context to interact with our database

##### Index

Now let's look at the `Index()` method

```csharp
public async Task<IActionResult> Index()
{
    return View(await _context.ToDos.ToListAsync());
}
```

We have already spoken about this earlier. What if we make it so this method returns  just`View()` instead? We get an `NullPReferenceException` thrown - the model is null!.

##### Create 

Look at our `Create` method - notice two things:

- The attribute, `[ValidateAntiForgeryToken]`
  - MVCs anti-forgery support which writes a unique value to an HTTP-only cookie and the same value written to the form, so when the page is submitted, an error is raised if that cookie value doesn't match the form value. 
  - This prevents cross site request forgeries. 
    - That is, a form from another site that posts to your site in an attempt to submit hidden content using an authenticated user's credentials. 
    - The attack involves tricking the logged in user into submitting a form, or by simply programmatically triggering a form when the page loads. 
- Notice the argument it takes `Create([Bind("Id,Title,Description,Complete,DateCreated")] ToDo toDo)`. 
  - The `[Bind]` attribute can be applied to class or a method parameter
  - Specifies which properties of a model should be included in the model binding
  - If we look in the Create view, we see that a model that is being used `@model SpartaToDo.Data.Models.ToDo` 
  - Once we put in all the relevant details of our new `ToDoItem`, this will be stored in the model in our RazorPage
  - When we create, the create method is called and the model is passed as an argument
  - The `[Bind]` attribute specifies the properties that we want to include in the model binding
  - If we delete the "DateCreated" from our model binding, nothing will change as in out `ToDo` model, we already set the default as `DateTime.Now`
    - Let's remove this from our `Bind` parameter
    - We can also remove it from `Create.cshtml` or make it readonly using the `readonly` HTML attribute
  -  `return View(toDo);`sets the Model in the `Create.cshtml` to the model we have chosen to (model binding). We then use data from this `toDo` object, which is bound to our Model in the associated Razor Page, in said Razor page.

##### Edit

There are to Edits - one which takes us to the edit page and another which will save the edits for us. We will focus on the latter.

Our `Edit` method is similar to our create, however, it has a condition `if(ModelSate.IsValid)` which indicates if it was possible to bind the incoming values from the request to the model correctly and whether any explicitly specified validation rules were broken during the model binding process.

It the `ModelState` is *not* valid, then we return the action `RedirectToAction(nameof(Index));` which gets the name of the `Index`, which is "Index" (we could type in the string "Index too"), and will redirect us to the ToDo page

##### Delete

Notice the two attributes above `DeleteConfirmed`:

```csharp
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
```

We have already explained the `[ValidateAntiFOrgeryToken]`. The `[HttpPost, ActionName("Delete")]` is saying "This is a post request" and when we click the "delete" button on this page, rather than the asp-action using our "Delete" method, it will use our `DeleteConfirmed` ,method which takes us to the delete page.

**Debug through to show**

#### Updating Controller using ModelViews

Our `ViewModel`

```csharp
using System.ComponentModel.DataAnnotations;

namespace SpartaToDo.Models
{
    public class ToDoViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        [Display(Name = "Complete ?")]
        public bool Complete { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Created")]
        public DateTime DateCreated { get; } = DateTime.Now;
		//ignore the ones below
        public int TasksNotComplete { get; set; }
        public int TasksComplete { get; set; }
    }
}

```

Generate a class called `Utils` in the controller folder:

```csharp
using Todo.Data;
using Todo.Data.Models;
using Todo.Models;

namespace Todo.Controllers
{
    public static class Utils
    {
        public static ToDoViewModel ToDoModelToToDoViewModel(ToDo todo) =>
            new ToDoViewModel
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                Complete = todo.Complete              
            };
    }
}

```



Our new `Index` method within the `ToDoController`

```csharp
public async Task<IActionResult> Index()
{
    var toDos = await _context.ToDos.ToListAsync();
    //Create a new list of "ModelView" types
    var toDosViewModelList = new List<ToDoViewModel>();
    

            //Iterate through our list of tDos and assign each one of their property values
            //to a new ToDoViewModel object and add each one to the toDoVIewModelList
            foreach (var toDo in toDos)
            {
                toDosViewModelList.Add(Utils.ToDoModelToToDoViewModel(toDo));
            }


	//Bind the toDosViewModelList to the the Model property in your Index view
    return View(toDosViewModelList);
}
```

> ALTERNATIVE
>
> 



Update the model in the `Index.cshtml` in the "Views >> ToDo"

```c#
@model IEnumerable<SpartaToDo.Models.ToDoViewModel>
```

Now when we run the application, it should still work the same as it did before.

> **EXERCISE**
>
> 1 hour
>
> Get trainees to update the other methods in the `ToDoController`  except for the `Delete` method (we will go through this one together). Examples below:
>
> `ToDOController`
>
> - `Details` Methods
>
>   ```c#
>    public async Task<IActionResult> Details(int? id)
>   {
>       if (id == null)
>       {
>           return NotFound();
>       }
>   
>       var toDo = await _context.ToDos
>           .FirstOrDefaultAsync(m => m.Id == id);
>   
>       if (toDo == null)
>       {
>           return NotFound();
>       }
>      var toDoViewModel = Utils.ToDoModelToToDoViewModel(toDo);
>       return View(toDoViewModel);
>   }
>   ```
>   
>   Update model in `Details.cshtml` to `@model SpartaToDo.Models.ToDoViewModel`
>   
> - `Edit`
>
>   ```csharp
>   public async Task<IActionResult> Edit(int id, [Bind("Title,Description,Complete,DateCreated")] ToDoViewModel toDoViewModel)
>   {
>       var toDo = _context.ToDos.Where(t => t.Id == id).FirstOrDefault();
>   
>       if (id != toDo.Id || toDo == null)
>       {
>           return NotFound();
>       }
>   
>       if (ModelState.IsValid)
>       {
>           try
>           {
>               toDo.Complete = toDoViewModel.Complete;
>               toDo.Description = toDoViewModel.Description;
>               toDo.Title = toDoViewModel.Title;
>               _context.Update(toDo);
>               await _context.SaveChangesAsync();
>           }
>           catch (DbUpdateConcurrencyException)
>           {
>               if (!ToDoExists(toDo.Id))
>               {
>                   return NotFound();
>               }
>               else
>               {
>                   throw;
>               }
>           }
>   
>           return RedirectToAction(nameof(Index));
>       }
>       return View(toDoViewModel);
>   }
>   ```
>
>   Update model in `Edit.cshtml` to `@model SpartaToDo.Models.ToDoViewModel`. Also remove the `<li>` attribute which contains date created 
>
> - `Edit`
>
>   ```csharp
>   // GET: ToDo/Edit/5
>   public async Task<IActionResult> Edit(int? id)
>   {
>       if (id == null)
>       {
>           return NotFound();
>       }
>   
>       var toDo = await _context.ToDos.FindAsync(id);
>       if (toDo == null)
>       {
>           return NotFound();
>       }
>       return View(Utils.ToDoModelToToDoViewModel(toDo));
>   }
>   ```
>
>   
>
> - `Create`
>
>   ```csharp
>   //Removed "Id" from the Bind attribute parameter (will be created automatically for us)
>   public async Task<IActionResult> Create([Bind("Title,Description,Complete")] ToDoViewModel toDoViewModel)
>   {
>       if (ModelState.IsValid)
>       {
>           var toDo = new ToDo 
>           { 
>               Title = toDoViewModel.Title,
>               Description = toDoViewModel.Description,
>               Complete = toDoViewModel.Complete
>           };
>   
>           _context.Add(toDo);
>           await _context.SaveChangesAsync();
>           return RedirectToAction(nameof(Index));
>       }
>       return View(toDoViewModel);
>   }
>   ```
>
>   Update model in `Create.cshtml` to `@model SpartaToDo.Models.ToDoViewModel`
>   
>   



## Updating Delete Function (OPTIONAL)

1. Comment out or delete the `DeleteConfirmed` method and update the `Delete` method:

   ```csharp
   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Delete(int id)
   {
       var toDo = await _context.ToDos.FindAsync(id);
       _context.ToDos.Remove(toDo);
       await _context.SaveChangesAsync();
       return RedirectToAction(nameof(Index));
   }
   ```

   

2. Update the button layout in `ToDo >> Index.htmlcs`. Update lines 46 - 50:

   ```html
       <td class = "btn-group-lg btn-group-vertical">
           <a asp-action="Edit" asp-route-id="@item.Id" class = "btn btn-primary">Edit</a> 
           <a asp-action="Details" asp-route-id="@item.Id" class = "btn btn-info">Details</a> 
           <a asp-action="Delete" asp-route-id="@item.Id" class = "btn btn-danger">Delete</a>
       </td>
   ```

3. Now if we run the application and delete, we get an error. We have a delete method, but it only respond to a Http post request! We are going to use a HTML helper (not a tag helper) to help us with this!

4. In "ToDo >> Index.cshtml" replace delete with this:

   ```html
   @using (Html.BeginForm("Delete", "ToDo", new {id = item.Id}))
   {
     <input type="submit" class = "btn btn-danger" value ="Delete"/> 
   }
   ```

   `BeginForm` helper generates a form tag for us, and we want to post this form. To Generate this form tag we can use this HTML helper. This Helper has several overloaded version. The one we used took the name of our actions ("Delete"), our controller name ("ToDo") and if that action method has any parameters (which it does). 

5. However, what if I want client side confirmation? All we need to do is to inject so JavaScript code:

   ```html
   @using (Html.BeginForm("delete", "ToDo", new {id = item.Id}))
   {
     <input type="submit" class = "btn btn-danger" value ="Delete" onclick="return confirm('Are you sure you wish to delete the To Do Item @item.Title?')"/> 
   }
   ```

6. Now let make things look a bit tidier:

   ```html
   <td class = "btn-group-vertical">
       <a asp-action="Edit" asp-route-id="@item.Id" class = "btn btn-primary">Edit</a> 
       <a asp-action="Details" asp-route-id="@item.Id" class = "btn btn-info">Details</a> 
       @*<input asp-for="1" asp-action="Delete" type="submit" value="Delete" type="submit" class="btn btn-danger"/> *@
      <a class = "btn btn-danger">
       @using (Html.BeginForm("delete", "ToDo", new {id = item.Id}))
       {
         <input type="submit" class = "border-0 bg-transparent" value ="Delete" onclick="return confirm('Are you sure you wish to delete the To Do Item @item.Title?')"/> 
       }
       </a>
   </td>
   ```

   And update the `<table>` class attribute:` <table class="table table-bordered">`

## Search Functionality (OPTIONAL)

Add the following HTML to "ToDo >> Index.cshtml" just below `<p><a asp-action="Create">Create New</a></p>` (i.e. under line 12):

```html
<form asp-controller="ToDo" asp-action="Index">
    <p>
        Search: <input type="text" name="SearchString" />
        <input type="submit" value="Filter" />
    </p>
</form>
```

Update `Index` method in `ToDoController`:

```csharp
public async Task<IActionResult> Index(String searchString)
{
    var toDos = await _context.ToDos.ToListAsync();
    var toDosViewModelList = new List<ToDoViewModel>();
    foreach (var toDo in toDos)
    {
        toDosViewModelList.Add(new ToDoViewModel
        {
            Id = toDo.Id,
            Title = toDo.Title,
            Description = toDo.Description,
            Complete = toDo.Complete,
        });

        if (!String.IsNullOrEmpty(searchString))
        {
            toDosViewModelList = toDosViewModelList.Where(t => t.Title.ToLower().Contains(searchString.ToLower()) ||
            t.Description.ToLower().Contains(searchString.ToLower())).ToList();
        }

    }
    return View(toDosViewModelList);
}
```

Now we can filter our results by any string!

## Displaying Data in charts (OPTIONAL)

There still needs to be some work done on this section. Potentially have a lesson on Google chart seperately?

However, this script works fine if you paste it in at the end of the `Index.htmlcs` file we've been working on

```html
<div>
<script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
<script type="text/javascript">
  google.charts.load("current", {packages:["corechart"]});
  google.charts.setOnLoadCallback(drawChart);
  function drawChart() {
    var data = google.visualization.arrayToDataTable([
      ['Status', 'Number'],
      ['Complete',     @Model.Where(x => x.Complete).Count()],
      ['Incomplete',   @Model.Where(x => !x.Complete).Count()]
    ]);

    var options = {
      title: 'To Do Completion',
      pieHole: 0.4,
    };

    var chart = new google.visualization.PieChart(document.getElementById('donutchart'));
    chart.draw(data, options);
  }
</script>
</div>
<div>
<div id="donutchart" style="width: 900px; height: 500px;"></div>
</div>

```



## Click Checkbox on screen (Work in progress)

>  Ignore for the time being - it's quite  hackneyed way of doing it. Redo walkthrough but with a `for` loop being used to update the table instead of the `foreach` loop



We want to display the To Do items together with an updatable checkbox to indicate whether the To Do is complete. There fore we need to get the model binder to associated each checkbox with a specific ToDo.

`In "Views >> ToDo >> Index.cshtml", update line 43`:

```html
<td>
    @Html.CheckBoxFor(modelItem => item.Complete)
</td>
```

The `CheckBoxFor` is strongly typed. Means, It will be always bounded with a model properties.

Create a new method in the `ToDoController`

```csharp
[HttpPost]
public async Task<IActionResult> Submit(int Id, bool complete)
{

    var selectedToDo = await _context.ToDos.Where(x => x.Id == Id).FirstOrDefaultAsync();
    selectedToDo.Complete = complete;
    _context.SaveChanges();
    return RedirectToAction(nameof(Index));
}
```



At the bottom of the HTML document, add:

```html
<br />
    <div>
          <input type="submit" asp-for="Submit" class = "btn btn-outline-dark" value ="Submit" onclick="return confirm('Are you sure you wish update the complete status of the selected items?')"/> 
    </div>

```

##  Adding Authentication



# Further Reading

Model Binding

https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-6.0

Tag Helpers:

https://docs.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms?view=aspnetcore-6.0

Displaying Data in charts:

https://docs.microsoft.com/en-us/aspnet/web-pages/overview/data/7-displaying-data-in-a-chart

https://canvasjs.com/asp-net-mvc-charts/doughnut-chart/