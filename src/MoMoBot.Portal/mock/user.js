export default {
    'POST /api/login/account': (req, res) => {
        setTimeout(() => {
            const { password, username, remember } = req.body;
            if (password === 'admin' && username === 'admin') {
                res.send({
                    status: 'ok',
                    userInfo: {
                        id: '10000',
                        username: 'zengande',
                        nickname: 'zeng ande'
                    },
                    token: {
                        access_token: 'ashddhasdhsadhashahash',
                        expires_in: 3600,
                        token_type: 'code',
                        refresh_token: 'hfdghfdh_sdfgwere'
                    }
                });
                return;
            }
            res.send({
                status: 'error'
            });
        }, 3000)
    }
}