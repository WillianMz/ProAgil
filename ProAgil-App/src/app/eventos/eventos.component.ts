import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-eventos',
  templateUrl: './eventos.component.html',
  styleUrls: ['./eventos.component.css']
})
export class EventosComponent implements OnInit {

  listaFiltro: string;
  get filtroLista(): string
  {
    return this.listaFiltro;
  }

  set filtroLista(value: string)
  {
    this.listaFiltro = value;
    this.eventosFiltrados = this.filtroLista ? this.filtrarEvento(this.filtroLista) : this.Eventos;
  }

  eventosFiltrados: any = [];

  Eventos: any = [];
  imagemLargura = 50;
  imagemMargem = 2;
  mostrarImagem = false;

  constructor(private http: HttpClient) { }

  ngOnInit()
  {
    this.getEventos();
  }

  filtrarEvento(filtrarPor: string)
  {
    filtrarPor = filtrarPor.toLocaleLowerCase();
    return this.Eventos.filter(evento =>
      {
        return evento.tema.toLocaleLowerCase().includes(filtrarPor);
      });
  }

  alternarImagem()
  {
    this.mostrarImagem = !this.mostrarImagem;
  }

  getEventos()
  {
    this.http.get('http://localhost:5000/api/values').subscribe(
      response => { this.Eventos = response; console.log(response); }, error => { console.log(error); }
    );
  }

}
