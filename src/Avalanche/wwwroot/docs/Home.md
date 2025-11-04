# Avalanche Documentation

Welcome to the **Avalanche** documentation.

## Overview

**Avalanche** is a web-based load testing framework designed to simulate traffic, evaluate performance, and analyze system behavior under stress conditions. It enables both interactive and automated execution of test scenarios.

## Architecture

Avalanche is composed of two core components:

- **Client Application**
- **Runner**

### Client Application

The **Client Application** serves as the primary interface for creating, executing, and monitoring load test scenarios.  
It provides functionality to:

- Define and execute load test scenarios  
- Store and manage execution results  
- Visualize performance metrics through a graphical interface  
- Expose a REST API used by external components such as the Runner  

The Client Application can execute scenarios independently and does not require the Runner to perform load tests.

### Runner

The **Runner** is a lightweight execution engine capable of running scenarios outside the Client Application environment.  
It is typically used for distributed or automated testing pipelines.

When configured with the Client Application’s API endpoint, the Runner transmits its execution results back to the Client for aggregation and visualization.  
If no API URL is provided, results are output directly to the console.

### Integration

- The Runner communicates with the Client via the exposed API.  
- The Client processes and stores the received results.  
- Both components can operate independently, but integration provides full visibility into test results.
