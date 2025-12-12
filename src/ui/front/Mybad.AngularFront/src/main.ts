import { AppComponent } from './app/app.component';
import { bootstrapApplication } from '@angular/platform-browser';
// import { AppModule } from './app/app.module';

import { provideHttpClient } from '@angular/common/http';
import { enableProdMode, importProvidersFrom, isDevMode } from '@angular/core';
import { AppRoutingModule } from './app/app-routing.module';

bootstrapApplication(AppComponent, 
    {
      providers: [
        importProvidersFrom(AppRoutingModule),
        provideHttpClient()
      ]
    }
  );
