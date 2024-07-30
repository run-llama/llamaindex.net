#!/bin/bash

# Install .NET Aspire Workload
# See documentation for more details
# https://learn.microsoft.com/dotnet/aspire/fundamentals/setup-tooling?tabs=linux&pivots=vscode
if command -v dotnet &> /dev/null
then
    echo "dotnet is installed."
    
    # Specify the workload you want to install
    WORKLOAD="aspire"

    # Update workloads
    sudo dotnet workload update

    # Install the workload
    sudo dotnet workload install $WORKLOAD

    echo "Workload '$WORKLOAD' has been installed."
else
    echo "dotnet is not installed. Please install dotnet first."
fi

# Download data files for examples
#!/bin/bash

# URL of the file to download
FILE_URL="https://arxiv.org/pdf/1706.03762"

# Directory where you want to place the downloaded file
TARGET_DIR="./samples/data"

# Name of the file after downloading
FILE_NAME="attention-is-all-you-need.pdf"

# Create the target directory if it doesn't exist
mkdir -p $TARGET_DIR

# Download the file and place it in the target directory
curl -o $TARGET_DIR/$FILE_NAME $FILE_URL

echo "File downloaded to $TARGET_DIR/$FILE_NAME"
