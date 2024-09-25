# External libraries used
#### The following nuget packages are required to run the application

- Moq
- AutoFixture
- Refit
- FluentValidation

# Testing

- I have included a Postman collection named: `CKO-PaymentGateway.postman_collection.json`
- You can run the collection and the collection variables are used to share data between each request

# Assumptions

- Could use any other libraries
- Do not over engineer it as of now
- Use of latest features in .net are allowed

# Some enhancements at top of my mind, which could be done in future

- Use result type or use functional programming - OneOf or Either etc
- Add centralized exception handling for Refit API exception
- Add support for multiple merchants
- Use an actual DB
- Provide support other payment methods such as bank transfer, wallet etc
