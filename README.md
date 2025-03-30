# Project Name

## Description
This project is a backend implementation for the Syncfusion PDF Viewer. It provides APIs to handle PDF rendering, text extraction, and other functionalities required for seamless PDF viewing. The backend is designed to integrate with the Syncfusion PDF Viewer component, enabling features like annotation, form filling, and text search.


## Prerequisites for .NET

Before proceeding with the installation, ensure you have the following prerequisites installed on your system:

1. **.NET SDK**: Download and install the latest version of the .NET SDK from the [official .NET website](https://dotnet.microsoft.com/download).
2. **Git**: Ensure Git is installed to clone the repository. You can download it from [Git's official website](https://git-scm.com/).
3. **Node.js and npm** (if required for frontend integration): Download and install from the [Node.js website](https://nodejs.org/).
4. **Database** (if applicable): Ensure the required database (e.g., SQL Server, PostgreSQL) is installed and properly configured.
5. **Docker** (optional): If you plan to use Docker, ensure Docker is installed and running on your system. You can download it from [Docker's official website](https://www.docker.com/).

Verify the installation by running the following commands:

```bash
dotnet --version
git --version
node --version
npm --version
docker --version
```

## Installation
Clone the repository:
   ```bash
   git clone https://github.com/daochoam/syncfusion-pdf-viewer.git
   cd your-repository
   ```
Before starting, ensure you have the following installed on your system:

  1. **Bash Shell**: Required to execute the `pdf_viewer.sh` script. Most Unix-based systems come with Bash pre-installed. For Windows, you can use Git Bash or WSL (Windows Subsystem for Linux).

  2. Execute the `pdf_viewer.sh` script:
  ```bash
    sh pdf_viewer.sh
  ```

  3. The `pdf_viewer.sh` script provides the following menu options:

  ```text
    1) Run project
    2) Restore project
    3) Build project
    4) Start container
    5) Run container
    6) Build container image
    7) Exit
  ```

### Using dotnet

Follow these steps to set up the project using dotnet:

  4. To restore the project, select option **2** from the menu. This will restore the project to its initial state, ensuring all dependencies and configurations are properly set up. You can execute the following command in the console: ``
    dotnet restore
  ``
  5. To run the project, select option **1** from the menu. Alternatively, you can execute the following command in the console: ``
    dotnet run
  ``
  6. The project will be publishing on port `5000`. You can access the API at `http://localhost:5000/api/pdf-viewer`.


### Using Docker
Follow these steps to set up the project using Docker:

  1. To create the container image, select option **6** from the menu. This will build the Docker image required for the project.
  2. To configure and run the container, select option  **5**  from the menu. This will start the container with the necessary configurations for the project.
  3. If you need to initialize the container, select option **4** from the menu. This will prepare the container for use.
  4. The container will be publishing on port `3000`. You can access the API at `http://localhost:3000/api/pdf-viewer`.

### Troubleshooting Syncfusion.EJ2.PdfViewer.PdfiumNative Error

If you encounter the following error:

```
The type initializer for Syncfusion.EJ2.PdfViewer.PdfiumNative threw an exception
```

This issue is typically caused by a missing `pdfium` dependency in the environment. To resolve this, follow the steps below based on your operating system:

  ```markdown
  1. Create a symbolic link for the missing library:
      sudo ln -s /usr/lib64/libdl.so.2 /usr/lib64/libdl.so
2. Install the required dependencies:
    # Fix for Debian-based Systems:
      sudo apt-get update
      sudo apt install libgdiplus
    # Fix for Fedora-based Systems:
      sudo dnf update
      sudo dnf install libgdiplus
  ```
