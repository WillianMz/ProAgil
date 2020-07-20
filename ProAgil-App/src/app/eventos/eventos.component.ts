import { Component, OnInit, TemplateRef } from '@angular/core';
import { EventoService } from '../_services/evento.service';
import { Evento } from '../_models/Evento';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { defineLocale } from 'ngx-bootstrap/chronos';
import { ptBrLocale } from 'ngx-bootstrap/locale';
import { ToastrService } from 'ngx-toastr';

defineLocale('pt-br', ptBrLocale);

@Component({
  selector: 'app-eventos',
  templateUrl: './eventos.component.html',
  styleUrls: ['./eventos.component.css']
})
export class EventosComponent implements OnInit {

  titulo = 'Eventos';
  eventosFiltrados: Evento[];
  evento: Evento;
  eventos: Evento[];
  imagemLargura = 50;
  imagemMargem = 2;
  mostrarImagem = false;
  registerForm: FormGroup;
  bodyDeletarEvento = '';
  listaFiltrada = '';
  dataEvento: string;

  modoSalvar = 'post';
  file: File;
  fileNameToUpdate: string;
  dataAtual: string;

  constructor(
    private eventoService: EventoService,
    private modalService: BsModalService,
    private fb: FormBuilder,
    private localService: BsLocaleService,
    private toastr: ToastrService
  ){
    this.localService.use('pt-br');
  }

  get filtroLista(): string{
    return this.listaFiltrada ;
  }

  set filtroLista(value: string){
    this.listaFiltrada  = value;
    this.eventosFiltrados = this.filtroLista ? this.filtrarEventos(this.filtroLista) : this.eventos;
  }

  ngOnInit(){
    this.validation();
    this.getEventos();
  }

  getEventos()
  {
    this.eventoService.getAllEventos().subscribe(
      (evtEventos: Evento[]) => {
        this.eventos = evtEventos;
        this.eventosFiltrados = this.eventos;
        // console.log(this.eventos);
        // this.toastr.info(`${this.eventos}`);
      }, error => {
        this.toastr.error(`Erro ao tentar carregar eventos: ${error}`);
      });
  }

  filtrarEventos(filtrarPor: string): Evento[]{
    filtrarPor = filtrarPor.toLocaleLowerCase();
    return this.eventos.filter(
      evento => evento.tema.toLocaleLowerCase().indexOf(filtrarPor) !== -1
    );
  }

  editarEvento(evento: Evento, template: any){
    this.modoSalvar = 'put';
    this.openModal(template);
    this.evento = Object.assign({}, evento);
    // guarda o nome do arquivo de img
    this.fileNameToUpdate = evento.imagemURL.toString();
    this.evento.imagemURL = '';
    this.registerForm.patchValue(this.evento);
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

  alternarImagem(){
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
      if (this.modoSalvar === 'post'){
        this.evento = Object.assign({}, this.registerForm.value);

        this.uploadImagem();

        this.eventoService.postEvento(this.evento).subscribe(
          (novoEvento: Evento) => {
            console.log(novoEvento);
            template.hide();
            this.getEventos();
            this.toastr.success('Inserido com sucesso!');
          },
          error => {
            this.toastr.error(`Erro ao inserir novo evento: ${error}`);
          }
        );
      }
      else{
        this.evento = Object.assign({eventoID: this.evento.eventoID}, this.registerForm.value);
        this.eventoService.putEvento(this.evento).subscribe(
          () => {
            template.hide();
            this.getEventos();
            this.toastr.success('Evento editado com sucesso!');
          },
          error => {
            this.toastr.error(`Erro ao editar evento: ${error}`);
          }
        );
      }
    }
  }

  confirmeDelete(template: any){
    this.eventoService.deleteEvento(this.evento.eventoID).subscribe(
      () => {
        template.hide();
        this.getEventos();
        this.toastr.success('Deletado com sucesso!', 'teste');
      },
      error => {
        this.toastr.error(`Erro ao deletar evento: ${error}`);
      }
    );
  }

  onFileChange(event){
    // console.log(event);
    const reader = new FileReader();

    // se tem imagem e tamanho nao minusculo
    if (event.target.files && event.target.files.lenght){
      // atribui
      this.file = event.target.files;
      console.log(this.file);
    }
  }

  uploadImagem(){
    // trata o nome do arquivo, pois o mesmo vem com diretorio incorreto
    // exemplo:
    // imagemURL = c:\\fakeFolder\img1.jpg; diretorio incorreto
    // nomeArquivo = [c:, fakeFolder, img1.jpg];
    if (this.modoSalvar === 'post'){
      const nomeArquivo = this.evento.imagemURL.split('\\', 3);
      this.evento.imagemURL = nomeArquivo[2];
      
      this.eventoService.postUpload(this.file, nomeArquivo[2])
        .subscribe(
          () => {
            this.dataAtual = new Date().getMilliseconds().toString();
            this.getEventos();
          }
        );
    }else{
      this.evento.imagemURL = this.fileNameToUpdate;
      this.eventoService.postUpload(this.file, this.fileNameToUpdate)
        .subscribe(
          () => {
            this.dataAtual = new Date().getMilliseconds().toString();
            this.getEventos();
          }
        );
    }
  }

}
