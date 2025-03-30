#!/bin/bash

# Función para limpiar la consola
console_clear() {
   docker stop pdf-viewer-app > /dev/null 2>&1
   clear
}

trap console_clear SIGINT SIGTERM
echo "1) Run project"
echo "2) Restore project"
echo "3) Build project"
echo "4) Start container"
echo "5) Run container"
echo "6) Build container image"
echo "7) Exit"
echo -n "Selected option: "
read OPTION

case $OPTION in
  1)
      dotnet run
      ;;
  2)
      dotnet restore
      ;;
  3)
      dotnet build
      ;;
  4)
      docker start pdf-viewer-app
      ;;
  5)
      docker run -d --name pdf-viewer-app -p 3000:3000 pdf-viewer:latest
      ;;
  6)
      docker build -t pdf-viewer .
      ;;
  7)
      echo "Saliendo del script."
      SALIR=1
      ;;
  *)
      echo "Opción inválida"
      ;;
  esac
