# 📢 Multichannel Notification System for Industrial Machines  

This repository contains the development and implementation of the **Multichannel Notification System**, created during a thesis project at **ADIGE-SYS**, part of the **BLM Group**, a global leader in the production and digitalization of laser systems for large-scale sheet metal and tube processing.  

---

## 📖 Project Overview  

With the rapid evolution of technology and communication, the **Multichannel Notification System** was designed to modernize how industrial machines communicate their status to customers.  
The system leverages popular messaging platforms like **Telegram**, **Teams**, **Slack**, and **WhatsApp**, replacing the traditional email-based notifications used by ADIGE-SYS.  

### 🚀 Why This Project?  
The previous email-based notification system (using the **Ewon** industrial VPN router) had several limitations:  
- Lack of real-time interactivity.  
- Inefficient handling of instant notifications.  
- Limited integration with modern communication platforms.  
- Security concerns regarding sensitive data transmitted via email.  

The new system addresses these challenges by delivering **real-time, encrypted notifications** through messaging apps, ensuring a more secure and interactive communication channel.  

---

## 🛠️ Development Phases  

### **1️⃣ Problem Definition & Solution Design**  
- Collaborated with company stakeholders to analyze the need for a modern notification system.  
- Defined objectives to make communications faster and more accessible using messaging platforms customers already utilize daily.  

### **2️⃣ System Architecture Design**  
- Designed a **modular and scalable microservices-based architecture** to ensure extensibility.  
- Technologies used:  
  - **.NET 6** for API server development.  
  - **gRPC** for efficient, high-performance inter-service communication.  
  - **SQLite** for data management.  
  - **Docker** for containerization of the application.  

### **3️⃣ Implementation**  
- Developed using **gRPC**, leveraging **HTTP/2** and **Protocol Buffers** for API definition, ensuring cross-language compatibility, scalability, and efficiency.  
- Conducted a demonstration test on the **LS7 laser machine**, using **Telegram** as the primary communication channel.  
- Confirmed the system’s ability to:  
  - Register users securely.  
  - Send real-time notifications.  

---

## ✅ Final Deliverables  

- A fully functional notification system allowing clients to receive real-time updates via **Telegram**.  
- Support for additional features via a chatbot interface.  
- **Modular Architecture**: Easily extendable to integrate new channels like **WhatsApp**, **Microsoft Teams**, and **Slack**.  
- Improved customer communication and real-time monitoring of production processes.  

---

## 🌐 Future Work  

- Integration of additional messaging platforms.  
- Expansion of chatbot functionality to support advanced queries and interactions.  

This project demonstrates the potential of modern communication tools to revolutionize how businesses interact with their clients, ensuring efficiency, security, and scalability.  
