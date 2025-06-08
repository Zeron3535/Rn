# TinaKuaför PostgreSQL Migration Guide

## Overview
This project has been successfully migrated from SQLite to PostgreSQL for better scalability and Railway deployment compatibility.

## PostgreSQL Migration Changes

### 1. Database Provider
- **OLD**: SQLite (`Microsoft.EntityFrameworkCore.Sqlite`)
- **NEW**: PostgreSQL (`Npgsql.EntityFrameworkCore.PostgreSQL`)

### 2. Connection String Configuration
- **Development**: `Host=localhost;Database=tinaKuafor;Username=postgres;Password=password`
- **Production**: Uses Railway environment variables automatically

### 3. Environment Variables (Railway)
```
DATABASE_URL=postgresql://postgres:wblqkebAPYwRtLObwMfmBsWZqXLtoWXH@postgres.railway.internal:5432/railway
DATABASE_PUBLIC_URL=postgresql://postgres:wblqkebAPYwRtLObwMfmBsWZqXLtoWXH@hopper.proxy.rlwy.net:27081/railway
PGHOST=hopper.proxy.rlwy.net
PGPORT=27081
PGDATABASE=railway
PGUSER=postgres
PGPASSWORD=wblqkebAPYwRtLObwMfmBsWZqXLtoWXH
```

## Database Schema
The PostgreSQL database includes all the enhanced features:

### Core Tables
- **Services** - Kuaför hizmetleri
- **ServiceCategories** - Hizmet kategorileri
- **Appointments** - Randevular (ReminderSent, CalendarReminderCreated alanları ile)
- **AppointmentServices** - Randevu-hizmet ilişkileri
- **BusinessHours** - Çalışma saatleri
- **ClosedDays** - Kapalı günler
- **Testimonials** - Müşteri yorumları
- **SiteSettings** - Site ayarları

### Enhanced Tables
- **FinancialRecords** - Gelir/Gider takibi
- **Documents** - Belge/Not yönetimi

## Deployment Steps

### 1. Local Development
```bash
# PostgreSQL sunucusu çalıştır
# Veritabanı: tinaKuafor
# Kullanıcı: postgres
# Şifre: password

dotnet ef database update
dotnet run
```

### 2. Railway Deployment
```bash
# Railway'e deploy
railway login
railway link
railway deploy

# Migration'ları Railway'de çalıştır
railway run dotnet ef database update
```

### 3. Environment Variables Setup
Railway dashboard'da aşağıdaki değişkenleri ayarla:
- `TELEGRAM_API_KEY`: Telegram bot API anahtarı
- `TELEGRAM_CHAT_ID`: Telegram chat ID'leri
- `ADMIN_PASSWORD`: Admin panel şifresi

## Key Features
✅ **Modern Dashboard** - Finansal takip ve belge yönetimi
✅ **Advanced Appointment System** - Gelişmiş randevu sistemi
✅ **Telegram Bot Automation** - Otomatik hatırlatmalar ve bildirimler
✅ **Financial Management** - Gelir/gider takibi
✅ **Document Management** - Belge/not yönetimi
✅ **PostgreSQL Database** - Ölçeklenebilir veritabanı

## Database Migration Notes
- SQLite INTEGER AUTOINCREMENT → PostgreSQL SERIAL
- SQLite TEXT → PostgreSQL VARCHAR/TEXT
- Foreign key constraints maintained
- Indexes and relationships preserved
- Entity Framework migrations for PostgreSQL

## Production URLs
- **Main Site**: https://your-railway-app.railway.app/anasayfa
- **Admin Panel**: https://your-railway-app.railway.app/Admin
- **Health Check**: https://your-railway-app.railway.app/health

## System Requirements
- .NET 8.0
- PostgreSQL 13+
- Entity Framework Core 9.0.0
- Npgsql.EntityFrameworkCore.PostgreSQL 9.0.0

## Support
Sistem artık tamamen PostgreSQL tabanlı ve Railway'de production-ready durumda! 