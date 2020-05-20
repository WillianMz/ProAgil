import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Evento } from '../_models/Evento';

@Injectable({
  providedIn: 'root'
})
export class EventoService {

  baseURL = 'http://localhost:5000/api/evento';

  constructor(private Http: HttpClient) { }

  getAllEventos(): Observable<Evento[]>{
    return this.Http.get<Evento[]>(this.baseURL);
  }

  getEventoByTema(tema: string): Observable<Evento[]>{
    return this.Http.get<Evento[]>('${this.baseURL}/getByTema/${tema}');
  }

  getEventoByID(id: number): Observable<Evento>{
    return this.Http.get<Evento>('${this.baseURL}/${id}');
  }

}
