##Field-Worker-Assistant


####Team Members
* `Preeti Maske` - Authoring Offline Services
* `Jennifer Nery` - Development With Offline Services
* `Thad Tilton` - Presenting & Quality Assurance
* `Christopher Hill` Development UX Design
* `Hisham Massih` - Development With Offline Services
* `Rich Zwaap` - Developement UX Design



####Offline Tech

* TPK Basemap ( Reference Location )
* Service Items ( Geodatabase )
* Offline Geocoding ( Turning Points into Street Addresses )
* Offline Routing ( Getting directions to Service Items)

####Test Data
We can either use Hosted Feature service or a FeatureService published locally on the ArcGIS Server.
Hosted Feature Service: http://services.arcgis.com/pmcEyn9tLWCoX7Dm/arcgis/rest/services/HackathonSR/FeatureServer/0
OR
http://win2008r2rx3:6080/arcgis/rest/services/HackathonSR/FeatureServer/0

####The Use Case

--Overview--
Field Worker Assistant helps a field crew stay on top of tasks assigned to them for the day. It was designed with crews for a small city government in mind, but
is applicable to any scenario where tasks are assigned that have a spatial component (must be accessed via a street network, i.e.). Designed to be available
when disconnected, the app allows the user to pull the day's assignments before leaving the office in the morning and to push progress on each task back to the 
database when the day is over. Tasks that weren't completed will be carried over to the following day.

--Check Out--
The first step in using the application is to check out the assigned tasks for the day. This would occur when the application is connected (at the office first thing in the
morning, e.g.). After logging into the application, a query is made to retreive tasks that have been assigned to the current user and that have a status of "incomplete".
The user is able to view the assigned tasks and to plan the day's work according to task priority, estimated time to complete, and optimal route. The tasks are listed in 
the planned order in which they will be visited, and a route is calculated to all work locations for the day. 

--Planning the Work Day--
Tasks can be reordered at any time to alter the order in which they will be visited and routes will automatically be recalculated when they are. This assures that route 
considerations can be taken into account in addition to task priority. Additionally, an individual task in the list may be selected at any time and a route calculated from 
the current location to the work address. This provides fluidity to planning the user's work for the day.

--Completing Tasks--
When a work location is reached, the application provides a task worksheet to record progress on the task. Specifically, the user can record the arrival time, the time the 
work was completed, a description of the work performed, and can update the status ("complete", e.g.). When the worksheet is saved, it is stored locally and will be 
synchronized with the parent database at the end of the day.

--Synchronizing--
When the work day is complete and the crew returns to the office (or other connected environment), data describing the work day is synchronized with the main database.
Information regarding all work tasks is updated, such as the status of the day's tasks, the time spent, and a description of the work performed. Tasks that were not completed
will be rolled over for the next day's assignment and may have the priority bumped up if time is a factor. The crew's activity for the day is also updated in the database, showing
their gps position and stops throughout the day.
