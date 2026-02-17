# Route Fitness - Gym Management System 🏋️‍♂️

A comprehensive Full-Stack web application built with **ASP.NET Core MVC** designed to streamline gym operations, manage memberships, and schedule training sessions.

## 🚀 Overview
Route Fitness is an all-in-one management solution for fitness centers. It provides administrative tools to track members, manage trainer schedules, and handle various membership plans through a secure, role-based web interface.

## 🛠️ Tech Stack
* **Framework:** ASP.NET Core MVC 8.0 (or your version)
* **Language:** C#
* **Database:** Microsoft SQL Server
* **ORM:** Entity Framework Core (Code First)
* **Security:** ASP.NET Core Identity (Authentication & Authorization)
* **Frontend:** HTML5, CSS3, JavaScript, Bootstrap

## ✨ Key Features
* **Dynamic Dashboard:** Real-time statistics on active members, trainers, and upcoming sessions.
* **Member Management:** Register new members with detailed health metrics (Height, Weight, Blood Type).
* **Trainer Management:** Full CRUD operations for trainers, including specialization tracking (Yoga, CrossFit, etc.).
* **Membership System:** Manage tiered subscription plans (Basic, Standard, Premium) with automated start/end date tracking.
* **Session Scheduling:** Book and manage training classes with capacity limits and attendance tracking.
* **Security:** * Secure Admin Login.
    * Role-based access control to protect administrative routes.
    * Customized "Access Denied" handling for unauthorized users.

## 📸 Screenshots
*(You can add your video screenshots here later)*
| Login Page | Member List | Membership Plans |
| :--- | :--- | :--- |
| ![Login]() | ![Members]() | ![Plans]() |

## 🏗️ Database Schema
The system utilizes a relational database designed for performance and integrity:
* **Identity Tables:** For user security and role management.
* **Members Table:** Linked to health data and active subscriptions.
* **Sessions Table:** Manages the relationship between trainers, categories, and booked members.

## ⚙️ How to Run
1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/AhmedKhaled/Route-Fitness-System.git](https://github.com/AhmedKhaled/Route-Fitness-System.git)
    ```
2.  **Update Connection String:** Modify `appsettings.json` with your local SQL Server credentials.
3.  **Apply Migrations:**
    ```bash
    dotnet ef database update
    ```
4.  **Run the application:**
    ```bash
    dotnet run
    ```

---
Developed by **Mohamed Raafat** – [LinkedIn](https://www.linkedin.com/in/mohamed-raafat-7b4788247/)
For [Demo](https://www.linkedin.com/posts/mohamed-raafat-7b4788247_dotnet-aspnetcore-webdevelopment-activity-7419006701707800576-ipdt?utm_source=share&utm_medium=member_desktop&rcm=ACoAAD0pRngBimaWESQGBYsr0kB8H5LFLVIoFMc)
