import { Component, OnInit, TemplateRef } from '@angular/core';
import { EventoService } from '../_services/evento.service';
import { Evento } from '../_models/Evento';
import { BsModalService } from 'ngx-bootstrap/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { defineLocale } from 'ngx-bootstrap/chronos';
import { ptBrLocale } from 'ngx-bootstrap/locale';
import { JsonPipe } from '@angular/common';
import { templateJitUrl } from '@angular/compiler';

defineLocale('pt-br', ptBrLocale);

@Component({
  selector: 'app-eventos',
  templateUrl: './eventos.component.html',
  styleUrls: ['./eventos.component.css']
})
export class EventosComponent implements OnInit {

  eventosFiltrados: Evento[];
  evento: Evento;
  eventos: Evento[];
  imagemLargura = 50;
  imagemMargem = 2;
  mostrarImagem = false;
  registerForm: FormGroup;
  bodyDeletarEvento = '';

  listaFiltro = '';
  modoSalvar = 'post';

  constructor(
    private eventoService: EventoService,
    private modalService: BsModalService,
    private fb: FormBuilder,
    private localService: BsLocaleService
  )
  {
    this.localService.use('pt-br');
  }

  get filtroLista(): string
  {
    return this.listaFiltro;
  }

  set filtroLista(value: string)
  {
    this.listaFiltro = value;
    this.eventosFiltrados = this.filtroLista ? this.filtrarEvento(this.filtroLista) : this.eventos;
  }

  editarEvento(evento: Evento, template: any){
    this.modoSalvar = 'put';
    this.openModal(template);
    this.evento = evento;//evento atual
    this.registerForm.patchValue(evento);
  }

  novoEvento(template: any){
    this.modoSalvar = 'post';
    this.openModal(template);
  }

  excluirEvento(evento: Evento, template: any){
    this.openModal(template);
    this.evento = evento;
    this.bodyDeletarEvento = `Tem certeza que deseja excluir o Evento: ${evento.tema}, Código: ${evento.eventoID}`;
  }

  openModal(template: any){
    this.registerForm.reset();
    template.show();
  }

  ngOnInit()
  {
    this.validation();
    this.getEventos();
  }

  filtrarEvento(filtrarPor: string): Evento[]{
    filtrarPor = filtrarPor.toLocaleLowerCase();
    return this.eventos.filter(
      evento => evento.tema.toLocaleLowerCase().indexOf(filtrarPor) === -1
    );
  }

  alternarImagem()
  {
    this.mostrarImagem = !this.mostrarImagem;
  }

  validation(){
    this.registerForm = this.fb.group({
      tema: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]],
      local: ['', Validators.required],
      dataEvento: ['', Validators.required],
      qtdPessoas: ['', [Validators.required, Validators.max(120000)]],
      telefone: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      imagemURL: ['', Validators.required]
    });
  }

  salvarAlteracao(template: any)
  {
    if (this.registerForm.valid)
    {
      if(this.modoSalvar == 'post')
      {
        this.evento = Object.assign({}, this.registerForm.value);
        this.eventoService.postEvento(this.evento).subscribe(
          (novoEvento: Evento) => {
            console.log(novoEvento);
            template.hide();
            this.getEventos();
          },
          error => { console.log(error); }
        );
      }
      else
      {
        this.evento = Object.assign({eventoID: this.evento.eventoID}, this.registerForm.value);
        this.eventoService.putEvento(this.evento).subscribe(
          () => {
            template.hide();//fecha form modal
            this.getEventos();//atualiza grid de eventos
          },
          error => { console.log(error); }
        );
      }
    }
  }

  confirmeDelete(template: any){
    this.eventoService.deleteEvento(this.evento.eventoID).subscribe(
      () => {
        template.hide();
        this.getEventos();
      },
      error => {
        console.log(error);
      }
    )
  }

  getEventos()
  {
    this.eventoService.getAllEventos().subscribe(
      (objEvento: Evento[]) => {
      this.eventos = objEvento;
      this.eventosFiltrados = this.eventos;
      console.log(objEvento);
    }, error => {
      console.log(error);
    });
  }

}
