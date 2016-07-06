/*MIT License
*
*Copyright (c) 2016 Ricardo Santos
*
*Permission is hereby granted, free of charge, to any person obtaining a copy
*of this software and associated documentation files (the "Software"), to deal
*in the Software without restriction, including without limitation the rights
*to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*copies of the Software, and to permit persons to whom the Software is
*furnished to do so, subject to the following conditions:
*
*The above copyright notice and this permission notice shall be included in all
*copies or substantial portions of the Software.
*
*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
*SOFTWARE.
*/


using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using IdentityServer3.Contrib.Nhibernate.Entities;
using IdentityServer3.Contrib.Nhibernate.Enums;

namespace IdentityServer3.Contrib.Nhibernate.NhibernateConfig.MappingOverrides
{
    public class TokenMappingOverride : IAutoMappingOverride<Token>
    {
        public void Override(AutoMapping<Token> mapping)
        {
            mapping.Map(e => e.JsonCode).Length(4001);

            mapping.Map(e => e.Key).Not.Nullable();
            mapping.Map(e => e.TokenType).Not.Nullable().CustomType<TokenType>();

            mapping.Map(e => e.Key).UniqueKey("UK_KEY_TYPE");
            mapping.Map(e => e.TokenType).UniqueKey("UK_KEY_TYPE").Index("I_SUB_CLT_TYPE");

            mapping.Map(e => e.SubjectId).Index("I_SUB_CLT_TYPE");
            mapping.Map(e => e.ClientId).Index("I_SUB_CLT_TYPE");
        }
    }
}
