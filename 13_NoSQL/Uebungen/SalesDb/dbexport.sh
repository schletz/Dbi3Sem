cd /home
for collection in customers products orders 
do
    mongoexport --authenticationDatabase=admin --uri="mongodb://root:1234@localhost:27017/salesDb" --collection=$collection --out=$collection.json
done
