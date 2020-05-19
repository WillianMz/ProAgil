import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class EventoService {

 constructor(private Http: HttpClient) { }

  getEvento(){
    return this.Http.get();
  }


}
