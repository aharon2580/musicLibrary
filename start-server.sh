#!/bin/bash

# -------------------------------
# Script to auto-start MSSQL container
# and then run the .NET API with watch
# -------------------------------

CONTAINER_NAME="oneproject-mssql"

echo "Checking if MSSQL container '$CONTAINER_NAME' is running..."

# מנסה להרים את הקונטיינר, אם כבר רץ זה לא יזיק
if sudo docker ps --format '{{.Names}}' | grep -q "^${CONTAINER_NAME}$"; then
    echo "Container '$CONTAINER_NAME' is already running."
else
    echo "Starting container '$CONTAINER_NAME'..."
    sudo docker start $CONTAINER_NAME >/dev/null
    echo "Container started."
fi

echo "Launching .NET API with watch..."
dotnet watch run
