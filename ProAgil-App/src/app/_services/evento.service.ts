import { Injectable } from '@angular/core';
import { HttpClient, HttpHandler, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Evento } from '../_models/Evento';

@Injectable({
  providedIn: 'root'
})
export class EventoService {

  baseURL = 'http://localhost:5000/api/evento';

  constructor(private Http: HttpClient) { 

  }

  getAllEventos(): Observable<Evento[]>{
    return this.Http.get<Evento[]>(this.baseURL);
  }

  getEventoByTema(tema: string): Observable<Evento[]>{
    return this.Http.get<Evento[]>(`${this.baseURL}/getByTema/${tema}`);
  }

  getEventoByID(id: number): Observable<Evento>{
    return this.Http.get<Evento>(`${this.baseURL}/${id}`);
  }

  postEvento(evento: Evento){
    return this.Http.post(this.baseURL, evento);
  }

  putEvento(evento: Evento){
    return this.Http.put(`${this.baseURL}/${evento.eventoID}`, evento);
  }

  deleteEvento(id: number){
    return this.Http.delete(`${this.baseURL}/${id}`);
  }

  postUpload(file: File, name: string){
    //const fileToUpload = file[0] as File;
    const fileToUpload = <File> file[0];
    const formData = new FormData();
    formData.append('file', fileToUpload, name);

    return this.Http.post(`${this.baseURL}/upload`, formData);
  }

}
