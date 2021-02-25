import { storeService } from '../services/storage.service';
import { IIdentity } from '../@types/IIdentity';
import { IUserInfo } from 'src/@types/IUserInfo';
import { authorize, resetAuthority } from '../services/identity.service';
import router from 'umi/router';

const getInitialState = (): IIdentity => {
    const defaultUser: IIdentity = {
        isAuthenticated: false,
        userInfo: {
            id: '',
            email: '',
            nickname: '',
            avatar: ''
        }
    }
    const userInfo: IUserInfo = storeService.retrieve("UserData", null);
    const token = storeService.retrieve("Token", null);
    const isAuthenticated: boolean = token !== null && token !== '';
    const result = {
        ...defaultUser,
        isAuthenticated: (isAuthenticated && userInfo != null),
        userInfo: { ...userInfo }
    };
    return result;
}

const initialState: IIdentity = getInitialState();

export default {
    namespace: 'identity',
    state: {
        ...initialState
    },
    effects: {
        *login({ payload }: { payload: any }, { call, put }: { call: any, put: any }) {
            // const response = {
            //     status: 'ok', userInfo: {
            //         id: '10000',
            //         username: 'zengande',
            //         nickname: 'zeng ande',
            //         avatar: 'data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wCEAAkGBxAODxAPERAVFhARDxAQEBgVEhAVEBUQFRIWFxUZGBcZHSggGBolHRUWIz0iJzUrLjAwGB8zRDMsNygtLisBCgoKBwcNDgcHDisZHxkrKysrKysrKysrKysrKysrKysrKysrKysrKysrKysrKysrKysrKysrKysrKysrKysrK//AABEIAOEA4QMBIgACEQEDEQH/xAAcAAEAAgIDAQAAAAAAAAAAAAAABwgEBgEDBQL/xABOEAABAwIBCAQICgcECwAAAAABAAIDBBEFBgcSEyExQWEiUXGBCBQjMlKRobEVJDNCYnKCkqLBNUNTY3N0s2SytNEXJTRVZYOTo8LS8P/EABQBAQAAAAAAAAAAAAAAAAAAAAD/xAAUEQEAAAAAAAAAAAAAAAAAAAAA/9oADAMBAAIRAxEAPwCcUREBERAREQEREBERAREQEREBERAREQEREBERAREQEREBERAREQEREBERAREQEREBERARcEqHc4OedtLI6mw5rJHsOjJM+7og4bwxoI0z9Ld2oJjRVNqc6ONSEk17xfg1sTAOwBqx/wDSNjP+8Z/vD/JBbpFUdmcjGW7sQm7y0+8L0KHO7jcR21YkHVJFCR6w0H2oLUIoDwTP5UNsKukjeOLoXOjdb6rtIE94UoZJZw8OxWzIZtGY/qpbMl7hez/skoNsREQEREBERAREQEREBERAREQEREBERARFi4piEVLDJUTPDIomF73HcGj3nlxQeZlllTT4RSuqpySLhsbG205JDua2/rvwAVc8qc6WJ4i8hszoISbNjgc5uzhpPHScfUOS6sr8oKrKXE2NijcQXamjiv5rCdpPAONrk8LDgFOebzNrS4RG2R7Wy1hA1krhcNPFsQPmjnvPsQVydQYmWl5hrC21ySyoIt1k2XVh2TVdUlgho536Zs0iJ+gTu84jRA5kq5qIPEwTJmkpoImNpIGuaxofoxM8+w0jci523XpfB8P7GP8A6bP8lkogxH4ZTu2GCMjnGw/kvMr8i8MqPlaCnPPUsa77zQCveRBF2PZkMNnBNO6SnfttY6yK/Njjf1EKH8s83tfgztY9unAHDRnivoA32aXGN27fs5lWxXxLE17S1zQ5rgWuBALSDvBB3hBX7N5nlmpy2mxImWDY1s1iZ2fX/aN5+d2qe8Or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'
            //     }, token: ''
            // }
            const response = yield call(authorize, payload);
            // Login successfully
            const { userInfo, token, expires_at } = response;
            yield put({
                type: 'authorized',
                payload: {
                    userInfo
                }
            });
            storeService.store('IsAuthorized', true);
            storeService.store('TokenExpire', expires_at);
            storeService.store('UserData', userInfo);
            storeService.store('Token', token);
            router.push('/');
            //}
        },
        *logout(action: any, { call, put }: { call: any, put: any }) {
            yield put({
                type: 'unauthorized'
            })
            resetAuthority();
        }
    },
    reducers: {
        'authorized'(state: any, { payload }: { payload: IIdentity }) {
            return {
                isAuthenticated: true,
                userInfo: {
                    ...payload.userInfo
                }
            }
        },
        'unauthorized'() {
            return {
                isAuthenticated: false,
                userInfo: {
                    id: null,
                    username: null,
                    nickname: null
                }
            }
        }
    }
}