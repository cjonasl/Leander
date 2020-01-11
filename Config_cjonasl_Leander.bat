git config --local user.name cjonasl
git config --local user.email jonas5669@outlook.com

rem use set-url if origin exists already
git remote set-url origin git@github.com:cjonasl/Leander.git

rem use "add" if origin does not exist
rem git remote set-url origin git@github.com:cjonasl/Leander.git

git config core.sshCommand "ssh -i ~/.ssh/RsaKeycjonasl -F /dev/null"

