conn = new Mongo();
db = conn.getDB("COSMOS");
db.COSMOSUsers.insert({ "_id" : "Admin", "Active" : true, "Deleted" : false, "Email" : null, "LoginTimeUTC" : null, "Password" : "p@ssw0rd", "PasswordExpiryDateUTC" : null });