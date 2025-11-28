import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { IndexComponent } from './index/index.component';

// Here we should probably create the routes for the app - like paths
// So the app can be navigated like from /home, to /matchup etc
const routes: Routes = [
  { path: '', component: IndexComponent },
  // { path: 'wards', component: WardsComponent },
  // { path: 'matchups', component: MatchupsComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
