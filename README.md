# Syncfusion PDF-Viewer

<div>
<a href="https://www.linux.org/" target="_blank"><img style="margin: 10px" src="https://raw.githubusercontent.com/daochoam/Data-Bases/main/icons/linux.svg" alt="Linux" height="50" /></a>  
<a href="https://www.gnu.org/software/bash/" target="_blank"><img style="margin: 10px" src="https://raw.githubusercontent.com/daochoam/Data-Bases/7ebf3b247ec2a05d050ed0f1af37e7accac58a57/icons/bash.svg" alt="bash" height="50" /></a>
<a href="https://dotnet.microsoft.com/" target="_blank"><img style="margin: 10px" src="https://raw.githubusercontent.com/daochoam/Data-Bases/7ebf3b247ec2a05d050ed0f1af37e7accac58a57/icons/dotnet.svg" alt="dotnet" height="50" /></a>  
<a href="https://www.syncfusion.com/" target="_blank"><img style="margin: 10px" src="https://raw.githubusercontent.com/daochoam/Data-Bases/refs/heads/main/icons/Syncfusion.png" alt="syncfusion" height="50" /></a>
<a href="https://www.docker.com/" target="_blank"><img style="margin: 10px" src="https://raw.githubusercontent.com/daochoam/Data-Bases/7ebf3b247ec2a05d050ed0f1af37e7accac58a57/icons/docker.svg" alt="docker" height="50" /></a> 
<a href="https://docs.docker.com/compose/" target="_blank"><img style="margin: 10px" src="https://raw.githubusercontent.com/daochoam/Data-Bases/refs/heads/main/icons/dockerCompose.png" alt="docker" height="50" /></a>  
</div>

## Description
This project is a backend implementation for the Syncfusion PDF Viewer. It provides APIs to handle PDF rendering, text extraction, and other functionalities required for seamless PDF viewing. The backend is designed to integrate with the Syncfusion PDF Viewer component, enabling features like annotation, form filling, and text search.

- **Syncfusion Libraries**:
  - **PDF** (`Syncfusion.Pdf.Net.Core`) - PDF generation and manipulation.
  - **PDF Viewer** (`Syncfusion.EJ2.PdfViewer.AspNet.Core.Linux`) - PDF viewer for Linux.

## Prerequisites for .NET

Before proceeding with the installation, ensure you have the following prerequisites installed on your system:

1. **.NET SDK**: Download and install the latest version of the .NET SDK from the [official .NET website](https://dotnet.microsoft.com/download).
2. **Git**: Ensure Git is installed to clone the repository. You can download it from [Git's official website](https://git-scm.com/).
3. **Docker** (optional): If you plan to use Docker, ensure Docker is installed and running on your system. You can download it from [Docker's official website](https://www.docker.com/).
5. **Docker Compose**: If you plan to use Docker Compose, ensure it is installed. You can download it from [Docker Compose's official documentation](https://docs.docker.com/compose/install/).

Verify the installation by running the following commands:

```bash
dotnet --version
git --version
docker version
docker compose version
```

## 📄 Installed Packages:  
This project is built on **.NET 8.0** and uses the following packages:  
  

| 📦 Package                                             | Requested Version | Resolved Version | Description |
|-----------------------------------------------------|------------------|------------------|-------------|
| **DotNetEnv**                                      | 3.1.1            | 3.1.1            | Load environment variables from `.env` files in .NET applications. |
| **Microsoft.AspNetCore.Mvc.NewtonsoftJson**        | 8.0.5            | 8.0.5            | Support for `Newtonsoft.Json` in **ASP.NET Core MVC**. |
| **Newtonsoft.Json**                                | 13.0.3           | 13.0.3           | JSON handling library for .NET. |
| **NLog.Extensions.Logging**                        | 5.4.0            | 5.4.0            | Integration of **NLog** with the .NET logging system. |
| **SkiaSharp**                                      | 3.116.1          | 3.116.1          | 2D graphics library based on **Skia**. |
| **SkiaSharp.NativeAssets.Linux**                   | 3.116.1          | 3.116.1          | Native dependencies for **SkiaSharp** on Linux. |
| **Swashbuckle.AspNetCore**                         | 8.0.0            | 8.0.0            | Tool for generating **Swagger** documentation in **ASP.NET Core**. |
| **Syncfusion.EJ2.PdfViewer.AspNet.Core.Linux**     | 28.1.37          | 28.1.37          | **Syncfusion** PDF viewer for Linux in **ASP.NET Core**. |
| **Syncfusion.Licensing**                            | 29.1.33          | 29.1.33          | **Syncfusion** licensing library for .NET applications. |
| **Syncfusion.Pdf.Net.Core**                        | 29.1.33          | 29.1.33          | **Syncfusion** library for handling PDFs in .NET. |


## 🌐 Environment Variables

To configure the project, you need to create a `.env` file in the root directory with the following variables:

```plaintext
LISTEN_PORT=
ASPNETCORE_ENVIRONMENT=
CERTIFICATE_PATH=
CERTIFICATE_PASSWORD=
SYNCFUSION_LICENSE_KEY=
```

#### Explanation of Variables:
- **LISTEN_PORT** (optional): The port on which the application will listen (default is `8080`).
- **ASPNETCORE_ENVIRONMENT** (optional): Specifies the environment for the application (e.g., `development`, `production`), (default is `development`).
- **CERTIFICATE_PATH** (optional): Path to the SSL certificate file (used for `.pfx` files).
- **CERTIFICATE_PASSWORD** (optional): Password for the SSL certificate. Required if **CERTIFICATE_PATH** is provided.
- **SYNCFUSION_LICENSE_KEY** (optional): License key for Syncfusion libraries.

#### ⚙️ Mandatory `.env` Configuration for Docker

The `.env` file is **mandatory** for the correct execution of the `docker-compose` and `Dockerfile`. Ensure that all required environment variables are properly defined in the `.env` file before building or running the Docker containers.

Ensure the `.env` file is not committed to version control by adding it to your `.gitignore` file.

## 🛠️ Installation and Execution
Clone the repository:
   ```bash
   git clone https://github.com/daochoam/syncfusion-pdf-viewer.git pdf-viewer
   cd  pdf-viewer
   ```

  1. Restore the project dependencies by running the following command:
  ```bash
    dotnet restore
  ```
  2. To run the project, use the following command:
  ```bash
    dotnet run
  ```
  3. Once the containers are running, access the API at the default port `8080` or the port specified in the environment variable `LISTEN_PORT`:
  ```
    http://localhost:<LISTEN_PORT>/api/pdf-viewer
  ```
  
#### Using `pdf_viewer.sh`
Before starting, ensure you have the following installed on your system:

  1. **Bash Shell**: Required to execute the `pdf_viewer.sh` script. For Windows, you can use Git Bash or WSL (Windows Subsystem for Linux).

  2. Execute the `pdf_viewer.sh` script:

  The `pdf_viewer.sh` script provides the following menu options:

  ```text
    1) Run project
    2) Restore project
    3) Build project
    4) Start container
    5) Run container
    6) Build container image
    7) Exit
  ```

##### Using dotnet

Follow these steps to set up the project using dotnet:

  1. To restore the project, select option **2** from the menu. This will restore the project to its initial state, ensuring all dependencies and configurations are properly set up.
  2. To run the project, select option **1** from the menu. Alternatively, you can execute the following command in the console:
  3. The project will be publishing on port `8080` or the port specified in the environment variable `LISTEN_PORT`:
  ```
    http://localhost:<LISTEN_PORT>/api/pdf-viewer
  ```

##### Using docker
Follow these steps to set up the project using docker:

  1. To create the container image, select option **6** from the menu. This will build the Docker image required for the project.
  2. To configure and run the container, select option  **5**  from the menu. This will start the container with the necessary configurations for the project.
  3. If you need to initialize the container, select option **4** from the menu. This will prepare the container for use.
  4. The container will be publishing on port `8080` or the port specified in the environment variable `LISTEN_PORT`:   
  ```
    http://localhost:<LISTEN_PORT>/api/pdf-viewer
  ```

#### Using docker-compose

  1. Create a `.env` file in the root directory with the required variables as described in the **Environment Variables** section.
  2. Execute the following command to start the development environment:
  ```bash
    docker compose --profile development up
  ```
  3. Once the containers are running, access the API at the default port `8080` or the port specified in the environment variable `LISTEN_PORT`:
  ```
    http://localhost:<LISTEN_PORT>/api/pdf-viewer
  ```

### ⚠️ Troubleshooting PdfViewer.PdfiumNative Error

If you encounter the following error:

```
The type initializer for Syncfusion.EJ2.PdfViewer.PdfiumNative threw an exception
```

This issue is typically caused by a missing `pdfium` dependency in the environment. To resolve this, follow the steps below based on your operating system:

  ```markdown
  1. Create a symbolic link for the missing library:
      # sudo ln -s /usr/lib64/libdl.so.2 /usr/lib64/libdl.so
  2. Install the required dependencies:
     Fix for Debian-based Systems:
      # sudo apt-get update
      # sudo apt install libgdiplus
     Fix for Fedora-based Systems:
      # sudo dnf update
      # sudo dnf install libgdiplus
  ```
