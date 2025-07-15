# Swimming Academy API - Deployment Guide

## Overview
This guide provides step-by-step instructions for deploying the Swimming Academy API to both Docker containers and traditional server environments.

## Prerequisites
- .NET 8.0 SDK (for local development)
- Docker and Docker Compose (for containerized deployment)
- Smarterasp.net SQL Server database access

## Database Setup

### Smarterasp.net Database Configuration
The application is configured to use the Smarterasp.net SQL Server database:
- **Data Source**: SQL1004.site4now.net
- **Initial Catalog**: db_abab0e_omarsafyna
- **User Id**: db_abab0e_omarsafyna_admin
- **Password**: Set via environment variable `DB_PASSWORD`

**Important**: Replace `YOUR_DB_PASSWORD` with your actual Smarterasp.net database password.

## Docker Deployment

### Quick Start
1. Set your database password in the environment:
   ```bash
   export DB_PASSWORD=your_actual_password
   ```

2. Build and run using Docker Compose:
   ```bash
   docker-compose up --build
   ```

3. The API will be available at: `http://localhost:8080`

### Manual Docker Build
1. Build the Docker image:
   ```bash
   docker build -t swimming-academy-api .
   ```

2. Run the container with environment variables:
   ```bash
   docker run -p 8080:80 \
     -e ASPNETCORE_ENVIRONMENT=Production \
     -e ASPNETCORE_URLS=http://+:80 \
     -e DB_PASSWORD=your_actual_password \
     -e JWT_KEY=your_jwt_secret \
     swimming-academy-api
   ```

## Server Deployment

### Traditional Server Setup
1. Publish the application:
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. Copy the published files to your server

3. Set environment variables:
   ```bash
   export ASPNETCORE_ENVIRONMENT=Production
   export ASPNETCORE_URLS=http://+:80
   export DB_PASSWORD=your_actual_password
   export JWT_KEY=your_jwt_secret
   ```

4. Run the application:
   ```bash
   dotnet SwimmingAcademy.dll
   ```

### IIS Deployment
1. Install ASP.NET Core Hosting Bundle
2. Create an IIS application pool
3. Deploy the published files to IIS
4. Set environment variables in IIS:
   - `DB_PASSWORD` = your_actual_password
   - `JWT_KEY` = your_jwt_secret

## Configuration

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Set to `Production` for production deployment
- `ASPNETCORE_URLS`: Set to `http://+:80` for HTTP-only deployment
- `DB_PASSWORD`: Your Smarterasp.net database password
- `JWT_KEY`: Your JWT secret key (optional, defaults to development key)

### Connection String
The application uses the following connection string format:
```
Data Source=SQL1004.site4now.net;Initial Catalog=db_abab0e_omarsafyna;User Id=db_abab0e_omarsafyna_admin;Password=${DB_PASSWORD};TrustServerCertificate=true;Encrypt=true;Connection Timeout=30;Command Timeout=30;MultipleActiveResultSets=True
```

**Important Notes:**
- `Data Source` and `Initial Catalog` are the Smarterasp.net format
- `Encrypt=true` is required for Smarterasp.net
- `TrustServerCertificate=true` handles SSL certificate validation
- `MultipleActiveResultSets=True` allows multiple active result sets
- Password is replaced from environment variable `DB_PASSWORD`

## Health Checks
- Health endpoint: `GET /health`
- Ping endpoint: `GET /ping`
- Swagger UI: `GET /swagger`

## Troubleshooting

### Common Issues

1. **500 Internal Server Error**
   - Check database connectivity to Smarterasp.net
   - Verify `DB_PASSWORD` environment variable is set
   - Check application logs

2. **Database Connection Issues**
   - Ensure Smarterasp.net SQL Server is accessible
   - Verify username and password
   - Check firewall settings and network connectivity
   - Verify the connection string format

3. **Environment Variable Issues**
   - Ensure `DB_PASSWORD` is set correctly
   - Check that environment variables are available to the application
   - For Docker, verify environment variables in docker-compose.yml

4. **HTTPS Redirection Issues**
   - The application now handles HTTPS redirection conditionally
   - For Docker deployment, HTTPS redirection is disabled

### Logs
- Application logs are available in the console output
- For Docker: `docker logs <container-id>`
- For IIS: Check Event Viewer

## Security Considerations

1. **Database Credentials**
   - Use environment variables for database passwords
   - Never commit passwords to source control
   - Use strong, unique passwords

2. **JWT Configuration**
   - Update the JWT key in production using `JWT_KEY` environment variable
   - Use a strong, random secret key

3. **HTTPS**
   - Configure SSL certificates for production
   - Enable HTTPS redirection when SSL is available

## Monitoring

### Health Checks
The application includes built-in health checks:
- `GET /health` - Returns application status
- `GET /ping` - Simple connectivity test

### Logging
- Application logs are configured for different environments
- Production logging is set to Information level
- Database command logging is set to Warning level

## Support
For issues or questions, check the application logs and health endpoints first. The application includes comprehensive error handling and logging for troubleshooting. 