#Simple web application on .net core 2.1

Authorizes user to execute manually certain sql stored procedures

Database contains one table and two procedures.
Table has three counters, counting button clicks.

Text parameter is passed to counters' text field.

odd counter is green, even is red

proc Test1 reads, Test2 writes, thus passing the parameters works good
