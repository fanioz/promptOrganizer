#!/bin/bash
# Test Coverage Report Generation Script for Unix/Linux/macOS
# This script generates comprehensive test coverage reports using Coverlet and ReportGenerator

set -e

# Default parameters
CONFIGURATION="Debug"
OUTPUT_PATH="./coverage"
REPORT_TYPE="Html"
INCLUDE_HISTORY=false
FAIL_BELOW_THRESHOLD=false
LINE_COVERAGE_THRESHOLD=80
BRANCH_COVERAGE_THRESHOLD=75
METHOD_COVERAGE_THRESHOLD=80

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --configuration)
            CONFIGURATION="$2"
            shift 2
            ;;
        --output-path)
            OUTPUT_PATH="$2"
            shift 2
            ;;
        --report-type)
            REPORT_TYPE="$2"
            shift 2
            ;;
        --include-history)
            INCLUDE_HISTORY=true
            shift
            ;;
        --fail-below-threshold)
            FAIL_BELOW_THRESHOLD=true
            shift
            ;;
        --line-threshold)
            LINE_COVERAGE_THRESHOLD="$2"
            shift 2
            ;;
        --branch-threshold)
            BRANCH_COVERAGE_THRESHOLD="$2"
            shift 2
            ;;
        --method-threshold)
            METHOD_COVERAGE_THRESHOLD="$2"
            shift 2
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

echo -e "\033[32mStarting Test Coverage Report Generation...\033[0m"

# Create output directory if it doesn't exist
mkdir -p "$OUTPUT_PATH"

# Clean previous coverage results
echo -e "\033[33mCleaning previous coverage results...\033[0m"
rm -rf "$OUTPUT_PATH"/*

# Run tests with coverage collection
echo -e "\033[33mRunning tests with coverage collection...\033[0m"
dotnet test \
    --configuration "$CONFIGURATION" \
    --collect:"XPlat Code Coverage" \
    --results-directory:"$OUTPUT_PATH" \
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover \
    DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude="[xunit.*]*" \
    DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByAttribute="Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute" \
    DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByFile="**/Migrations/**"

# Find the coverage file
COVERAGE_FILE=$(find "$OUTPUT_PATH" -name "coverage.cobertura.xml" -type f | head -n 1)
if [ -z "$COVERAGE_FILE" ]; then
    echo -e "\033[31mNo coverage files found!\033[0m"
    exit 1
fi

echo -e "\033[32mFound coverage file: $COVERAGE_FILE\033[0m"

# Generate detailed coverage report
echo -e "\033[33mGenerating detailed coverage report...\033[0m"
REPORT_GENERATOR_ARGS=(
    "-reports:$COVERAGE_FILE"
    "-targetdir:$OUTPUT_PATH/report"
    "-reporttypes:$REPORT_TYPE"
    "-sourcedirs:./src"
    "-plugins:RiskHotspots"
    "-assemblyfilters:+PromptOrganizer.*"
    "-classfilters:-PromptOrganizer.Tests.*"
    "-filefilters:-*/Migrations/*"
)

if [ "$INCLUDE_HISTORY" = true ]; then
    REPORT_GENERATOR_ARGS+=("-historydir:$OUTPUT_PATH/history")
fi

dotnet reportgenerator "${REPORT_GENERATOR_ARGS[@]}"

# Generate summary report
echo -e "\033[33mGenerating coverage summary...\033[0m"
SUMMARY_ARGS=(
    "-reports:$COVERAGE_FILE"
    "-targetdir:$OUTPUT_PATH/summary"
    "-reporttypes:TextSummary"
    "-sourcedirs:./src"
    "-assemblyfilters:+PromptOrganizer.*"
)

dotnet reportgenerator "${SUMMARY_ARGS[@]}"

# Display coverage summary
echo -e "\033[36m\n=== COVERAGE SUMMARY ===\033[0m"
cat "$OUTPUT_PATH/summary/Summary.txt"

# Check coverage thresholds
if [ "$FAIL_BELOW_THRESHOLD" = true ]; then
    echo -e "\033[33m\nChecking coverage thresholds...\033[0m"
    
    # Parse coverage data from XML using xmllint or python
    if command -v xmllint >/dev/null 2>&1; then
        LINE_COVERAGE=$(xmllint --xpath "string(//coverage/@line-rate)" "$COVERAGE_FILE")
        BRANCH_COVERAGE=$(xmllint --xpath "string(//coverage/@branch-rate)" "$COVERAGE_FILE")
        LINE_COVERAGE=$(echo "$LINE_COVERAGE * 100" | bc -l | xargs printf "%.2f")
        BRANCH_COVERAGE=$(echo "$BRANCH_COVERAGE * 100" | bc -l | xargs printf "%.2f")
    elif command -v python3 >/dev/null 2>&1; then
        LINE_COVERAGE=$(python3 -c "
import xml.etree.ElementTree as ET
import sys
tree = ET.parse('$COVERAGE_FILE')
root = tree.getroot()
rate = float(root.get('line-rate', 0))
print(f'{rate * 100:.2f}')
")
        BRANCH_COVERAGE=$(python3 -c "
import xml.etree.ElementTree as ET
import sys
tree = ET.parse('$COVERAGE_FILE')
root = tree.getroot()
rate = float(root.get('branch-rate', 0))
print(f'{rate * 100:.2f}')
")
    else
        echo -e "\033[31mNeither xmllint nor python3 found. Cannot parse coverage thresholds.\033[0m"
        exit 1
    fi
    
    echo -e "Line Coverage: ${LINE_COVERAGE}% (Threshold: ${LINE_COVERAGE_THRESHOLD}%)"
    echo -e "Branch Coverage: ${BRANCH_COVERAGE}% (Threshold: ${BRANCH_COVERAGE_THRESHOLD}%)"
    
    # Convert to integers for comparison
    LINE_INT=${LINE_COVERAGE%.*}
    BRANCH_INT=${BRANCH_COVERAGE%.*}
    
    FAILED_THRESHOLDS=()
    if [ "$LINE_INT" -lt "$LINE_COVERAGE_THRESHOLD" ]; then
        FAILED_THRESHOLDS+=("Line Coverage")
    fi
    if [ "$BRANCH_INT" -lt "$BRANCH_COVERAGE_THRESHOLD" ]; then
        FAILED_THRESHOLDS+=("Branch Coverage")
    fi
    
    if [ ${#FAILED_THRESHOLDS[@]} -gt 0 ]; then
        echo -e "\033[31m\nFAILED: Coverage thresholds not met for: ${FAILED_THRESHOLDS[*]}\033[0m"
        exit 1
    else
        echo -e "\033[32m\nSUCCESS: All coverage thresholds met!\033[0m"
    fi
fi

echo -e "\033[32m\nCoverage report generation completed!\033[0m"
echo -e "\033[36mFull report available at: $(realpath "$OUTPUT_PATH/report/index.html")\033[0m"