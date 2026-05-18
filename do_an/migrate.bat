@echo off
dotnet ef migrations add MegaArchitectureUpgrade_FEFO_Cart
dotnet ef database update
