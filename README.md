# P2P-IronPython

A simple peer-to-peer application that allows users to post jobs in Python. To achieve this, we’re going to do a hybrid Web Service/.NET Remoting system.

The Applications
Main Components:

• An ASP.NET MVC Web Service that will host a list of client machines and save the
relevant information into a local database
• A desktop application (with NET Remoting) that uses the Web Service to find and
communicate with other clients.
• An ASP.NET CORE Website that communicates with the Web service to display the
information as a dashboard.
