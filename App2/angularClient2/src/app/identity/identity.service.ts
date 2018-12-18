import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from '../../environments/environment';

@Injectable({
providedIn: 'root'
})
export class IdentityService {

    constructor(private http: HttpClient) { }

    public get(): Observable<any> {
        return this.http.get<any>(`${environment.apiUrl}/api2/identity`);
    }

}
