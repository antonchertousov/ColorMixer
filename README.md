<details>
<summary>Expand for user interface description</summary>

# User interface
On application start user will see the empty list where is possible to create the objects using the context menu:
<br><br>
<img src="https://github.com/antonchertousov/ColorMixer/assets/121962913/6d702973-0cd6-420a-b931-5a1c2afee3fe" width="450">
<br><br>
By the context menu (right click anywhere) it is possible to create the new object:
<br><br>
<img src="https://github.com/antonchertousov/ColorMixer/assets/121962913/29666f50-e2f5-4c9a-ac1d-bf0801e69049" width="400">
<img src="https://github.com/antonchertousov/ColorMixer/assets/121962913/e83a3540-89e5-440c-944e-3f2e9351bc12" width="400">
<br><br>
The pictures above show one object and multiple objects created.
<br><br>
<table>
  <tbody>
    <td width="400">
      <img src="https://github.com/antonchertousov/ColorMixer/assets/121962913/67f47640-50f4-4b6d-8caa-23913fffebf3">
    </td>
    <td>
      For each object you can define the color by right click on the object and select “Set color” menu item.
    </td>
  </tbody>
</table>
<br>
<table>
  <tbody>
    <td>
The color picker window appears. There is possible to choose the color for the object just by clicking on picker area or select some of the predefined colors.
    </td>
    <td width="400">
      <img src="https://github.com/antonchertousov/ColorMixer/assets/121962913/1df93c97-681e-4fe5-a9db-33b960f526fc">
    </td>
  </tbody>
</table>
<br>
The next step could be the setup parent – child relations. Use the right click and context menu over the object to select the parent, then the same action to select the child:
<br>
<img src="https://github.com/antonchertousov/ColorMixer/assets/121962913/90b76976-81c0-4cf2-b80c-0b4df120bfda" width="400">
<img src="https://github.com/antonchertousov/ColorMixer/assets/121962913/44c13e27-e458-4b10-94d8-2905c77117d0" width="400">
<br><br>
<table>
  <tbody>
    <td width="600">
      Each time you select the object the information string appears at the top: you can see the object color in hex ARGB format and number of parents and children.
<br>
When the parent and child are set the children object color is automatically recalculated based on parents colors. The mixing ratio is equals to the number of parents. For example if object has 3 parents the each parent color ratio will be 1/3.
<br><br>
On the screen at the right, you see the mix of two parents.
    </td>
    <td>
      <img src="https://github.com/antonchertousov/ColorMixer/assets/121962913/bb5bf919-5a18-4745-9b9d-f365bad6524e">
    </td>
  </tbody>
</table>
<br><br> 
It is possible to setup any parent – child relation nodes number. Each time any of the parent’s color is changed the all dependent children colors will be updated automatically:
<br><br>
<img src="https://github.com/antonchertousov/ColorMixer/assets/121962913/14ad7042-7714-41ee-b946-c0695b93140a" width="600">
<br><br>
You always can see the statistics for the selected node by pressing the left mouse button. For example, on the screen above you see the information for the gray circle which have four parents and one child.
<br><br>
The usability of application was improved with drag and drop function available for any object (drag using left mouse click). The connection lines will be automatically updated in a real time. See the same configuration of nodes below:
<br><br>
<img src="https://github.com/antonchertousov/ColorMixer/assets/121962913/9af7b5e8-38a4-42d3-ab17-42c0bf6b7c9b" width="600">
<br><br>
The ‘Reset all’ button removes all the objects and connection lines from the drawing area.
<br><br>
</details>

<details>
<summary>Expand for technical part description</summary>

## Description
The application made with  .NET 4.8.1 and WPF components also using these external libraries:
-	WPF.ColorPicker v.5.0.0.1 (color picked component)
-	Mixbox v.2.0.0 (helper library for colors mixing)

Both components are downloaded as nuget packages.
Also, for arrow lines the code example from Charles Petzold © 2007 was used.

## Principles and considerations
Since the application doesn’t require multiple windows or complex models the one window with code behind approach was used. All xaml markup is located in MainWindow.xaml file and the interaction logic including handling of mouse, buttons and menu events are in the MainWindow.xaml.cs file.
Additional helpers for color mixing, lines drawing and recalculations for the drag-n-drop are in the ‘Helpers’ folder respectectively.
Property changed support was added to update the bindings in the xaml file.

## Data classes
The general idea of managing the objects and their relation is described below:
<img src="https://github.com/antonchertousov/ColorMixer/assets/121962913/4ac66f59-ca22-4479-8f58-02b040495f21">
<br><br>
The only one data class is used: Connection. The collection of ‘Connection’ objects contains the information about parent, child and their relation line identifiers. The parent and child objects have type of Ellipse, connection line type of ArrowLine.
Each time the new object created (Ellipse) it is added to the Canvas collection. To identify the object exactly the ‘Tag’ field contains the unique identifier.
Each time the parent-child relation is defined the new ‘ArrowLine’ object is created and added to the canvas collection. At the same time the new ‘Connection’ object is added to connections collection.

The ‘Connections’ collection allows to create the node structure and update the colors of child objects based on the parent colors. The diagram for the color update is described below (see implementation in UpdateChildColorsOnParentChange method):
<img src="https://github.com/antonchertousov/ColorMixer/assets/121962913/742a811d-ef72-45f4-b6a3-f785a3d88c77" width="600">
<br><br>

## The code region’s structure
To make the MainWindow.xaml.cs file more readable it was structured with the following self describing regions:
-	Private fields
-	Public properties
-	Mouse and button click actions
-	Context menu handling methods
-	Creating, moving objects and drawing connections
-	Colors mixing
-	Statistics
-	Public methods
-	Property changed support

## Building the app
If you want to build and run the application using the provided code the best option is to open the solution file in IDE like JB Rider or Microsoft Visual Studio, download Nuget packages using packages.config file and build the project:
<img src="https://github.com/antonchertousov/ColorMixer/assets/121962913/2931cbcd-4e5e-49b2-af1c-13c88532be71">
<br><br>
Then you can run it in debug or release configuration.

## Ideas for further development
The list of the features to implement is described below:

-	Improve selected object visibility (dotted line, or some other border type)
-	Implement deletion for the selected objects
-	Implement deletion for object relations (parent-child lines)
-	Implement/prevent circular references for the sets of objects
-	Improve UX for the context menu items (or use another approach to work with objects)
-	From the prevous item: show the color picker as side panel instead of separate window
-	Implement undo operations
-	Implement another color mixing approaches
-	Implement interactive object size change (for all or for separate)
-	Implement other types of objects to show (rectangle for example, or custom objects)
-	Save/load object configuration to the file
</details>
