Some decision making, thoughts and comments  <br/> <br/>

Db - Mssql is used, on cloud on my own Azure <br/>
Backend - .net core 9 web api <br/>
Front end - Blazor pages -- i thought to try this for the first time, plus i wanted an 'all in one' solution to avoid prerequisites <br/>
There is a db connection string and fixer.io api access key that are needed to change in the app settings for the implementation to work. The values will be included in the attackemtn in the response email of the task. <br/> <br/>

The application is working in some parts and not in others, going for Blazor pages was not a good desicion because of my lack of experience with it and issues with events probably that i couldnt figure out. <br/> <br/>

From the total tasks, 1a, 1b, 1c, 2a, 2b are implemented and there is some strategic decisionts taken for 2c, 2d but not implemented.  <br/>
By no means i would consider this something 'ready for code review', it is in a state that lets say 'the core' is done, and normally i would spend time policing and refactoring it. Improving areas mentioned below. <br/>
The main reason this was not done was due to time, as creating a robust 'structure' from scratch takes a lot of time and it seemed to me not the point of task, those things could remain open for discussion. <br/> <br/>

--DB-- <br/>
An approach to seperate the 'main' information needed (rates) and related inforamtion to that (date and currency). -- Fact - dimentions approach. <br/>
That may make the overal result a bit more complex (losing polymorphism) but that structure was chosen in mind with scalability and frequent db calls. <br/>
There is a unit of work implemented mostly to have total control over the daily update of the rates.  <br/>
This unit of work ideally should have a different structure as its not ideal for simple operations. <br/>
There is data in the cloud db. <br/>
With the connection string in the attacked file in the email it is possible to see in data studio the structure and data.  <br/> <br/>

--Structure-- <br/>
Overall i seperated the project in two sub projects but ideally it should have been even more separated in more subprojects. <br/>
Front end, model libraries, db handling and other areas could / should be different projects (with their own pipeline and git). <br/>

--Things lacking in need of improve-- <br/> <br/>
Logging in many areas should have been better. <br/>
Caching is not im place. <br/>
Resilience policies are not in place. <br/>
Validations on dto objects is not in place. <br/>
Response dtos should have a 'base' class they all inherit from that contains error codes, status etc. <br/>
More seperation of concerns should take place too, f.e. there are areas within the daily update that could be independent methods. <br/>
Application settings are very 'basic', ideally there should be a tokenizer that works within the pipelings so the values are always secret. <br/>
'Entities', i can see in that state of the project that entities could be untilized to 'dictate' how some operations should fuction, f.e. a currency convert can be an Entity. <br/>
Try catch improvements, there has been utilization of try catch, but in a very 'early' stage. <br/> <br/>

--Why not entity framework-- <br/>
I have some parts of implementation (like the unit of work) that exist within EF core, the main reason i did not pick EF is lack of experience with it, but also some 'distrust' i have with it. <br/>
In my experience, letting EF Core handle all db connection and operations fortfeits a lot of room for scalability and is not an ideal way to manage db. <br/>

