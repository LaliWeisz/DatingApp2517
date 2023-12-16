import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Dating application';
  users: any;
  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.http.get('http://localhost:5001/api/users').subscribe({
      next: response => this.users = response,
      error: err => console.log(err),
      complete: () => console.log('request has complete')
    })
  }
}
